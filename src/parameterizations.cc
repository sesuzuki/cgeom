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
#include "parameterizations.h"
}

namespace CGeom
{
    ///////////////////////////////////////////////
    // Mixed Integer Quadrangulation
    ///////////////////////////////////////////////

    CGEOM_PARAM_API void iglSeamlessIntegerGridParameterization(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t *outVertexCount, size_t *outFaceCount, double **outVertexCoords, int **outFaceIndexes)
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

        ///////////////////////////////////////////////////
        ///////////////////////////////////////////////////
        ///////////////////////////////////////////////////
        ///////////////////////////////////////////////////
        // qex_TriMesh triMesh;
        // qex_QuadMesh quadMesh;
        // unsigned int vertex_i, face_i;
        // qex_Point3* vertex;


        // triMesh.vertex_count = 4;
        // triMesh.tri_count = 2;

        // triMesh.vertices = (qex_Point3*)malloc(sizeof(qex_Point3) * 4);
        // triMesh.tris = (qex_Tri*)malloc(sizeof(qex_Tri) * 2);
        // triMesh.uvTris = (qex_UVTri*)malloc(sizeof(qex_UVTri) * 2);

        // triMesh.vertices[0] = (qex_Point3) { .x = {-1, 0, -1} };
        // triMesh.vertices[1] = (qex_Point3) { .x = {0, 0, 1} };
        // triMesh.vertices[2] = (qex_Point3) { .x = {1, 0, 1} };
        // triMesh.vertices[3] = (qex_Point3) { .x = {1, 0, 0} };

        // triMesh.tris[0] = (qex_Tri) { .indices = {0, 1, 2} };
        // triMesh.tris[1] = (qex_Tri) { .indices = {0, 2, 3} };
        // triMesh.uvTris[0] = (qex_UVTri) { .uvs = { (qex_Point2) { .x = {-.1, -.1} }, (qex_Point2) { .x = {1.1, -.1} }, (qex_Point2) { .x = {1, 1} } } };
        // triMesh.uvTris[1] = (qex_UVTri) { .uvs = { (qex_Point2) { .x = {-.1, -.1} }, (qex_Point2) { .x = {1, 1} }, (qex_Point2) { .x = {-.1, 1.1} } } };

        qex_TriMesh triMesh;
        qex_QuadMesh quadMesh;
        qex_Point3* vertex;

        triMesh.vertex_count = numVertices;
        triMesh.tri_count = numFaces;

        triMesh.vertices = (qex_Point3*)malloc(sizeof(qex_Point3) * numVertices);
        triMesh.tris = (qex_Tri*)malloc(sizeof(qex_Tri) * numFaces);
        triMesh.uvTris = (qex_UVTri*)malloc(sizeof(qex_UVTri) * numFaces);

        for(int i=0; i<numVertices; i++){
            triMesh.vertices[i] = (qex_Point3) { .x = { V(i,0), V(i,1), V(i,2) } }; //inCoords[i*3], inCoords[i*3+1], inCoords[i*3+2]} };    
        }

        for(int i=0; i<numFaces; i++){
            triMesh.tris[i] = (qex_Tri) { .indices = { (qex_Index) F(i,0), (qex_Index) F(i,1), (qex_Index) F(i,2) } };
            triMesh.uvTris[i] = (qex_UVTri) { .uvs = { (qex_Point2) { .x = { UV(FUV(i,0),0), UV(FUV(i,0),1) } }, (qex_Point2) { .x = { UV(FUV(i,1),0), UV(FUV(i,1),1) } }, (qex_Point2) { .x = { UV(FUV(i,2),0), UV(FUV(i,2),1) } } } };
        }

        //triMesh.uvTris[0] = (qex_UVTri) { .uvs = { (qex_Point2) { .x = {-.1, -.1} }, (qex_Point2) { .x = {1.1, -.1} }, (qex_Point2) { .x = {1, 1} } } };
        //triMesh.uvTris[1] = (qex_UVTri) { .uvs = { (qex_Point2) { .x = {-.1, -.1} }, (qex_Point2) { .x = {1, 1} }, (qex_Point2) { .x = {-.1, 1.1} } } };

        qex_extractQuadMesh(&triMesh, NULL, &quadMesh);

        // Parse quad mesh vertex data
        *outVertexCount = quadMesh.vertex_count * 3;
        auto sV = (quadMesh.vertex_count * 3) * sizeof(double);
        *outVertexCoords = static_cast<double *>(malloc(sV));

        std::vector<double> coord;
        for(int i=0; i<quadMesh.vertex_count; i++){
            auto vertex = quadMesh.vertices[i].x;
            coord.push_back(vertex[0]);
            coord.push_back(vertex[1]);
            coord.push_back(vertex[2]);
        }
        std::memcpy(*outVertexCoords, coord.data(), sV);

        // Parse quad mesh face data
        *outFaceCount = quadMesh.quad_count * 4;
        auto sF = (quadMesh.quad_count * 4) * sizeof(int);
        *outFaceIndexes = static_cast<int *>(malloc(sF));

        std::vector<int> faces;
        for(int i=0; i<quadMesh.quad_count; i++){
            auto quad = quadMesh.quads[i].indices;
            faces.push_back(quad[0]);
            faces.push_back(quad[1]);
            faces.push_back(quad[2]);
            faces.push_back(quad[3]);
        }
        std::memcpy(*outFaceIndexes, faces.data(), sF);


        free(triMesh.vertices);
        free(triMesh.tris);
        free(triMesh.uvTris);

        free(quadMesh.vertices);
        free(quadMesh.quads);
    }
}