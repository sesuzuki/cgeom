#include <igl/avg_edge_length.h>
#include <igl/barycenter.h>
#include <igl/comb_cross_field.h>
#include <igl/comb_frame_field.h>
#include <igl/compute_frame_field_bisectors.h>
#include <igl/cross_field_mismatch.h>
#include <igl/cut_mesh_from_singularities.h>
#include <igl/find_cross_field_singularities.h>
#include <igl/local_basis.h>
#include <igl/readOFF.h>
#include <igl/rotate_vectors.h>
#include <igl/copyleft/comiso/miq.h>
#include <igl/copyleft/comiso/nrosy.h>
#include <igl/PI.h>
#include <sstream>
#include <igl/serialize.h>
#include <qex.h>

extern "C"
{
#include "remeshing.h"
}

namespace CGeom
{
    ///////////////////////////////////////////////
    // Mixed Integer Quadrangulation
    ///////////////////////////////////////////////

    CGEOM_REMESH_API void iglQuadRemeshing(const int numVertices, const int numFaces, double *inCoords, int *inFaces)
    {
        // Build mesh
        Eigen::MatrixXd V;
        Eigen::MatrixXi F;
        V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        double gradient_size = 50;
        double iter = 0;
        double stiffness = 5.0;
        bool direct_round = 0;

        // Compute face barycenters
        Eigen::MatrixXd B;
        igl::barycenter(V, F, B);

        // Constrain one face
        Eigen::VectorXi b(1);
        Eigen::MatrixXd bc(1, 3);

        Eigen::MatrixXd X1,X2;                                                      // Cross field
        // Create a smooth 4-RoSy field
        Eigen::VectorXd S;
        igl::copyleft::comiso::nrosy(V, F, b, bc, Eigen::VectorXi(), Eigen::VectorXd(), Eigen::MatrixXd(), 4, 0.5, X1, S);

        // Find the orthogonal vector
        Eigen::MatrixXd B1, B2, B3;
        igl::local_basis(V, F, B1, B2, B3);
        X2 = igl::rotate_vectors(X1, Eigen::VectorXd::Constant(1, igl::PI / 2), B1, B2);

        // Always work on the bisectors, it is more general
        Eigen::MatrixXd BIS1, BIS2;                                                 // Bisector field
        igl::compute_frame_field_bisectors(V, F, X1, X2, BIS1, BIS2);

        // Comb the field, implicitly defining the seams      
        Eigen::MatrixXd BIS1_combed, BIS2_combed;                                   // Combed bisector
        igl::comb_cross_field(V, F, BIS1, BIS2, BIS1_combed, BIS2_combed);

        // Find the integer mismatches
        Eigen::Matrix<int, Eigen::Dynamic, 3> MMatch;                               // Per-corner, integer mismatches
        igl::cross_field_mismatch(V, F, BIS1_combed, BIS2_combed, true, MMatch);

        // Find the singularities         
        Eigen::Matrix<int, Eigen::Dynamic, 1> isSingularity, singularityIndex;      // Field singularities
        igl::find_cross_field_singularities(V, F, MMatch, isSingularity, singularityIndex);

        // Cut the mesh, duplicating all vertices on the seams
        Eigen::Matrix<int, Eigen::Dynamic, 3> Seams;                                // Per corner seams
        igl::cut_mesh_from_singularities(V, F, MMatch, Seams);

        // Comb the frame-field accordingly
        Eigen::MatrixXd X1_combed, X2_combed;                                       // Combed field
        igl::comb_frame_field(V, F, X1, X2, BIS1_combed, BIS2_combed, X1_combed, X2_combed);


        // Global parametrization
        Eigen::MatrixXd UV;
        Eigen::MatrixXi FUV;
        igl::copyleft::comiso::miq(V,
                F,
                X1_combed,
                X2_combed,
                MMatch,
                isSingularity,
                Seams,
                UV,
                FUV,
                gradient_size,
                stiffness,
                direct_round,
                iter,
                5,
                true);
    }
}