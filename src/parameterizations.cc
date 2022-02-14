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
#include <igl/serialize.h>
#include <igl/planarize_quad_mesh.h>
#include <sstream>
#include <qex.h>

extern "C"
{
#include "parameterizations.h"
}

namespace CGeom
{

    CGEOM_PARAM_API void cgeomParseMatrixXd(const Eigen::MatrixXd m, double **outData, size_t *outCount){
        *outCount = m.size();
        auto sF = *outCount * sizeof(double);
        *outData = static_cast<double *>(malloc(sF));
        std::memcpy(*outData, m.data(), sF);
    }

    CGEOM_PARAM_API void cgeomParseMatrixXi(const Eigen::MatrixXi m, int **outData, size_t *outCount){
        *outCount = m.size();
        auto sF = *outCount * sizeof(int);
        *outData = static_cast<int *>(malloc(sF));
        std::memcpy(*outData, m.data(), sF);
    }

    CGEOM_PARAM_API void cgeomNRosy(const int numVertices, const int numFaces, const int numConstraints, double *inCoords, int *inFaces, int *inConstrainedFaces, double *inConstrainedVectorFaces, 
                                    int degree, size_t *outX1CoordsCount, size_t *outX2CoordsCount, size_t *outBarycentersCoordsCount, size_t *outSingularitiesCount, double **outX1Coords, double **outX2Coords, double **outBarycentersCoords, double **outSingularities)
    {
        // Build mesh
        Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        // Compute face barycenters
        Eigen::MatrixXd B;
        igl::barycenter(V, F, B);

        // Constrain one face
        Eigen::VectorXi b = Eigen::Map<Eigen::VectorXi>(inConstrainedFaces, numConstraints, 1); 
        Eigen::MatrixXd bc = Eigen::Map<Eigen::MatrixXd>(inConstrainedVectorFaces,b.size(), 3);

        Eigen::MatrixXd X1,X2;                                                      // Cross field
        // Create a smooth 4-RoSy field
        Eigen::VectorXd S;
        igl::copyleft::comiso::nrosy(V, F, b, bc, Eigen::VectorXi(), Eigen::VectorXd(), Eigen::MatrixXd(), degree, 0.5, X1, S);

        // Find the orthogonal vector
        Eigen::MatrixXd B1, B2, B3;
        igl::local_basis(V, F, B1, B2, B3);
        X2 = igl::rotate_vectors(X1, Eigen::VectorXd::Constant(1, igl::PI / 2), B1, B2);

        // output vector field and barycenters
        cgeomParseMatrixXd(B, outBarycentersCoords, outBarycentersCoordsCount);
        cgeomParseMatrixXd(X1, outX1Coords, outX1CoordsCount);
        cgeomParseMatrixXd(X2, outX2Coords, outX2CoordsCount);
        cgeomParseMatrixXd(S, outSingularities, outSingularitiesCount);
    }

    CGEOM_PARAM_API void cgeomSeamlessIntegerGridParameterization(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double *inCoordsX1, double *inCoordsX2,
                                                                  double gradient_size, double stiffness, bool direct_round, size_t numIterations, size_t *outUVCount, size_t *outFUVCount, double **outUV, int **outFUV)
    {
        // Build mesh
        Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);
        Eigen::MatrixXd X1 = Eigen::Map<Eigen::MatrixXd>(inCoordsX1, numFaces, 3);
        Eigen::MatrixXd X2 = Eigen::Map<Eigen::MatrixXd>(inCoordsX2, numFaces, 3); 

        // Always work on the bisectors, it is more general
        Eigen::MatrixXd BIS1, BIS2;                                                 
        igl::compute_frame_field_bisectors(V, F, X1, X2, BIS1, BIS2);

        // Comb the field, implicitly defining the seams      
        Eigen::MatrixXd BIS1_combed, BIS2_combed;                                   
        igl::comb_cross_field(V, F, BIS1, BIS2, BIS1_combed, BIS2_combed);

        // Find the integer mismatches
        Eigen::Matrix<int, Eigen::Dynamic, 3> MMatch;                               
        igl::cross_field_mismatch(V, F, BIS1_combed, BIS2_combed, true, MMatch);

        // Find the singularities         
        Eigen::Matrix<int, Eigen::Dynamic, 1> isSingularity, singularityIndex;      
        igl::find_cross_field_singularities(V, F, MMatch, isSingularity, singularityIndex);

        // Cut the mesh, duplicating all vertices on the seams
        Eigen::Matrix<int, Eigen::Dynamic, 3> Seams;                                
        igl::cut_mesh_from_singularities(V, F, MMatch, Seams);

        // Comb the frame-field accordingly
        Eigen::MatrixXd X1_combed, X2_combed;                                       
        igl::comb_frame_field(V, F, X1, X2, BIS1_combed, BIS2_combed, X1_combed, X2_combed);

        // Global parametrization
        Eigen::MatrixXd UV;
        Eigen::MatrixXi FUV;
        igl::copyleft::comiso::miq(V, F, X1_combed, X2_combed, MMatch, isSingularity, Seams, UV, FUV,
                                   gradient_size, stiffness, direct_round, numIterations, 5, true);

        cgeomParseMatrixXd(UV, outUV, outUVCount);
        cgeomParseMatrixXi(FUV, outFUV, outFUVCount);    
    }

    CGEOM_PARAM_API void cgeomQuadMeshExtraction(const int inVertexCount, const int inTriasCount, const int inUVCount, const int inFUVCount, double *inCoords, int *inTrias, double *inUV, int *inFUV, 
                                              size_t *outVertexCount, size_t *outQuadsCount, double **outCoords, int **outQuads){

        // Build mesh
        Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords, inVertexCount, 3);
        Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inTrias, inTriasCount, 3);
        Eigen::MatrixXd UV = Eigen::Map<Eigen::MatrixXd>(inUV, inUVCount, 3);
        Eigen::MatrixXi FUV = Eigen::Map<Eigen::MatrixXi>(inFUV, inFUVCount, 3);

        // Initialize triangular mesh
        qex_TriMesh triMesh;
        qex_QuadMesh quadMesh;
        qex_Point3* vertex;

        triMesh.vertex_count = inVertexCount;
        triMesh.tri_count = inTriasCount;

        triMesh.vertices = (qex_Point3*)malloc(sizeof(qex_Point3) * inVertexCount);
        triMesh.tris = (qex_Tri*)malloc(sizeof(qex_Tri) * inTriasCount);
        triMesh.uvTris = (qex_UVTri*)malloc(sizeof(qex_UVTri) * inTriasCount);

        for(int i=0; i<inVertexCount; i++){
            triMesh.vertices[i] = (qex_Point3) { .x = { V(i,0), V(i,1), V(i,2) } };    
        }

        for(int i=0; i<inTriasCount; i++){
            triMesh.tris[i] = (qex_Tri) { .indices = { (qex_Index) F(i,0), (qex_Index) F(i,1), (qex_Index) F(i,2) } };
            triMesh.uvTris[i] = (qex_UVTri) { .uvs = { (qex_Point2) { .x = { UV(FUV(i,0),0), UV(FUV(i,0),1) } }, (qex_Point2) { .x = { UV(FUV(i,1),0), UV(FUV(i,1),1) } }, (qex_Point2) { .x = { UV(FUV(i,2),0), UV(FUV(i,2),1) } } } };
        }

        qex_extractQuadMesh(&triMesh, NULL, &quadMesh);

        // Parse quad mesh vertex data
        *outVertexCount = quadMesh.vertex_count * 3;
        auto sV = (quadMesh.vertex_count * 3) * sizeof(double);
        *outCoords = static_cast<double *>(malloc(sV));

        std::vector<double> coord;
        for(int i=0; i<quadMesh.vertex_count; i++){
            auto vertex = quadMesh.vertices[i].x;
            coord.push_back(vertex[0]);
            coord.push_back(vertex[1]);
            coord.push_back(vertex[2]);
        }
        std::memcpy(*outCoords, coord.data(), sV);

        // Parse quad mesh face data
        *outQuadsCount = quadMesh.quad_count * 4;
        auto sF = (quadMesh.quad_count * 4) * sizeof(int);
        *outQuads = static_cast<int *>(malloc(sF));

        std::vector<int> faces;
        for(int i=0; i<quadMesh.quad_count; i++){
            auto quad = quadMesh.quads[i].indices;
            faces.push_back(quad[0]);
            faces.push_back(quad[1]);
            faces.push_back(quad[2]);
            faces.push_back(quad[3]);
        }
        std::memcpy(*outQuads, faces.data(), sF);


        free(triMesh.vertices);
        free(triMesh.tris);
        free(triMesh.uvTris);

        free(quadMesh.vertices);
        free(quadMesh.quads);
    }

    CGEOM_PARAM_API void cgeomPlanarization(const int inVertexCount, const int inQuadsCount, double *inCoords, int *inQuads, const int iterations, const double threshold, size_t *outVertexCount, double **outCoords)
    {
        Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords, inVertexCount, 3);
        Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inQuads, inQuadsCount, 4);

        Eigen::MatrixXd outV;
        igl::planarize_quad_mesh(V, F, iterations, threshold, outV);

        cgeomParseMatrixXd(outV, outCoords, outVertexCount);
    }
}