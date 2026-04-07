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
#include <igl/copyleft/comiso/frame_field.h>
#include <igl/frame_field_deformer.h>
#include <igl/frame_to_cross_field.h>
#include <igl/copyleft/comiso/frame_field.h>
#include <sstream>
#include <qex.h>
#include <igl/cr_vector_laplacian.h>
#include <igl/cr_vector_mass.h>
#include <igl/crouzeix_raviart_cotmatrix.h>
#include <igl/crouzeix_raviart_massmatrix.h>
#include <igl/edge_midpoints.h>
#include <igl/edge_vectors.h>
#include <igl/average_from_edges_onto_vertices.h>
#include <igl/min_quad_with_fixed.h>
#include <igl/heat_geodesics.h>
#include <igl/remesh_along_isoline.h>
#include <igl/boundary_loop.h>
#include <igl/harmonic.h>
#include <igl/map_vertices_to_circle.h>
#include <igl/lscm.h>
#include "geometrycentral/surface/flip_geodesics.h"
#include "geometrycentral/surface/meshio.h"
#include "geometrycentral/surface/surface_mesh_factories.h"
#include "geometrycentral/utilities/vector3.h"
#include <igl/principal_curvature.h>
#include <igl/bounding_box_diagonal.h>

#include <directional/TriMesh.h>
#include <directional/PCFaceTangentBundle.h>
#include <directional/CartesianField.h>
#include <directional/power_field.h>
#include <directional/principal_matching.h>
#include <directional/combing.h>
#include <directional/power_to_raw.h>
#include <directional/setup_integration.h>
#include <directional/integrate.h>
#include <directional/setup_mesher.h>
#include <directional/mesher.h>
#include <signal.h>
#include <setjmp.h>
#include "instant_meshes_wrapper.h"

// Thread-local escape hatch so that assert()→abort() inside the Directional
// exact-arithmetic mesher (a submodule we cannot modify) gets converted into
// a std::runtime_error that the caller's try/catch can handle.
static thread_local sigjmp_buf  s_mesherJmpBuf;
static thread_local volatile sig_atomic_t s_mesherActive = 0;

static void mesherAbortHandler(int)
{
    if (s_mesherActive) {
        s_mesherActive = 0;
        siglongjmp(s_mesherJmpBuf, 1);   // jump back to the guarded call site
    }
    // Not our call — restore the default and re-raise so the process dies normally.
    signal(SIGABRT, SIG_DFL);
    raise(SIGABRT);
}

extern "C"
{
#include "processing.h"
}

namespace CGeom
{

    /////////////////////////////////////////////////////////////////////////////////////////
    // Geometry Central
    /////////////////////////////////////////////////////////////////////////////////////////

    CGEOM_PARAM_API void cgeomGetFlipGeodesics(int numVertices, int numFaces, int numPointOffsets, double *inCoords, int *inFaces, int *inPtsIndices, int *inPointOffset, double **outPointCoords, int **outPointOffsets, size_t *outNumCoords, size_t *outNumOffsets){
        // Build mesh
        Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        // construct geometry-central mesh types
        std::unique_ptr<geometrycentral::surface::ManifoldSurfaceMesh> mesh;
        std::unique_ptr<geometrycentral::surface::VertexPositionGeometry> geometry;
        std::tie(mesh, geometry) = geometrycentral::surface::makeManifoldSurfaceMeshAndGeometry(V, F);

        // Create a path network as a Dijkstra path between endpoints
        int startOff=0;
        std::unique_ptr<geometrycentral::surface::FlipEdgeNetwork> edgeNetwork;
        std::vector<double> outCoords;
        std::vector<int> outOffsets;
        int offsetPoints = 0;
        bool closed = false;
        for (int i = 0; i < numPointOffsets; i++)
        {
            int endOff = inPointOffset[i];
            int numGeoPts = (endOff - startOff);
            std::vector<geometrycentral::surface::Vertex> points;
            for (int j = 0; j < numGeoPts; j++) points.emplace_back(mesh->vertex(inPtsIndices[startOff + j]));
            startOff = endOff;

            edgeNetwork = geometrycentral::surface::FlipEdgeNetwork::constructFromPiecewiseDijkstraPath(*mesh, *geometry, points, closed);

            // Make the path a geodesic
            edgeNetwork->iterativeShorten();

            // Extract the result as a polyline along the surface
            edgeNetwork->posGeom = geometry.get();
            std::vector<std::vector<geometrycentral::Vector3>> polyline = edgeNetwork->getPathPolyline3D();

            for (size_t i = 0; i < polyline.size(); i++)
            {
                auto poly = polyline[i];
                for (size_t j = 0; j < poly.size(); j++)
                {
                    auto v = poly[j];
                    outCoords.push_back(v.x);
                    outCoords.push_back(v.y);
                    outCoords.push_back(v.z);
                    offsetPoints += 3;
                }
            }
            outOffsets.push_back(offsetPoints);

            //edgeNetwork->rewind();
        }
        
        *outNumCoords = outCoords.size();
        auto sizeCoords = (*outNumCoords) * sizeof(double);
        *outPointCoords = static_cast<double *>(malloc(sizeCoords));
        std::memcpy(*outPointCoords, outCoords.data(), sizeCoords);

        *outNumOffsets = outOffsets.size();
        auto sizeOffsets = outOffsets.size() * sizeof(int);
        *outPointOffsets = static_cast<int *>(malloc(sizeOffsets));
        std::memcpy(*outPointOffsets, outOffsets.data(), sizeOffsets);
    }

    CGEOM_PARAM_API void cgeomGetDijkstraPath(int numVertices, int numFaces, int numPaths, double *inCoords, int *inFaces, int *inStartIndices, int *inEndIndices, double **outPointCoords, int **outPointOffsets, size_t *outNumCoords, size_t *outNumOffsets){
                // Build mesh
        Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        // construct geometry-central mesh types
        std::unique_ptr<geometrycentral::surface::ManifoldSurfaceMesh> mesh;
        std::unique_ptr<geometrycentral::surface::VertexPositionGeometry> geometry;
        std::tie(mesh, geometry) = geometrycentral::surface::makeManifoldSurfaceMeshAndGeometry(V, F);

        // Create a path network as a Dijkstra path between endpoints
        std::unique_ptr<geometrycentral::surface::FlipEdgeNetwork> edgeNetwork;
        std::vector<double> outCoords;
        std::vector<int> outOffsets;
        int offsetPoints = 0;

        for (int i = 0; i < numPaths; i++)
        {
            // Create a path network as a Dijkstra path between endpoints
            geometrycentral::surface::Vertex vStart, vEnd;
            vStart = mesh->vertex(inStartIndices[i]);
            vEnd = mesh->vertex(inEndIndices[i]);

            edgeNetwork = geometrycentral::surface::FlipEdgeNetwork::constructFromDijkstraPath(*mesh, *geometry, vStart, vEnd);

            // Extract the result as a polyline along the surface
            edgeNetwork->posGeom = geometry.get();
            std::vector<std::vector<geometrycentral::Vector3>> polyline = edgeNetwork->getPathPolyline3D();

            for (size_t i = 0; i < polyline.size(); i++)
            {
                auto poly = polyline[i];
                for (size_t j = 0; j < poly.size(); j++)
                {
                    auto v = poly[j];
                    outCoords.push_back(v.x);
                    outCoords.push_back(v.y);
                    outCoords.push_back(v.z);
                    offsetPoints += 3;
                }
            }
            outOffsets.push_back(offsetPoints);
        }
        
        *outNumCoords = outCoords.size();
        auto sizeCoords = (*outNumCoords) * sizeof(double);
        *outPointCoords = static_cast<double *>(malloc(sizeCoords));
        std::memcpy(*outPointCoords, outCoords.data(), sizeCoords);

        *outNumOffsets = outOffsets.size();
        auto sizeOffsets = outOffsets.size() * sizeof(int);
        *outPointOffsets = static_cast<int *>(malloc(sizeOffsets));
        std::memcpy(*outPointOffsets, outOffsets.data(), sizeOffsets);
    }

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

    CGEOM_PARAM_API void cgeomParseVectorXi(const Eigen::VectorXi& v, int** outData, size_t* outCount)
    {
        const size_t n = (size_t)v.size();
        *outCount = v.size();
        auto sF = *outCount * sizeof(int);
        *outData = static_cast<int *>(malloc(sF));
        std::memcpy(*outData, v.data(), sF);
    }

    CGEOM_PARAM_API void cgeomParseStdVectorInt(const std::vector<int> m, int **outData, size_t *outCount){
        *outCount = m.size();
        auto sF = *outCount * sizeof(int);
        *outData = static_cast<int *>(malloc(sF));
        std::memcpy(*outData, m.data(), sF);
    }

    CGEOM_PARAM_API void cgeomParseStdVectorDouble(const std::vector<double> m, double **outData, size_t *outCount){
        *outCount = m.size();
        auto sF = *outCount * sizeof(double);
        *outData = static_cast<double*>(malloc(sF));
        std::memcpy(*outData, m.data(), sF);
    }

    CGEOM_PARAM_API void cgeomNRosy(const int numVertices, const int numFaces, const int numConstraints, double *inCoords, int *inFaces, 
                                    int *inConstrainedFaces, double *inConstrainedVectorFaces, int degree,
                                    size_t *outX1CoordsCount, size_t *outX2CoordsCount, size_t *outSingularitiesCount, 
                                    double **outX1Coords, double **outX2Coords, double **outSingularities)
    {
        // Build mesh
        Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        // Compute face barycenters
        Eigen::MatrixXd B;
        igl::barycenter(V, F, B);

        // Hard constraints
        Eigen::VectorXi b = Eigen::Map<Eigen::VectorXi>(inConstrainedFaces, numConstraints, 1); 
        Eigen::MatrixXd bc = Eigen::Map<Eigen::MatrixXd>(inConstrainedVectorFaces, numConstraints, 3);

        Eigen::MatrixXd X1,X2;
        // Create a smooth 4-RoSy field
        Eigen::VectorXd S;
        igl::copyleft::comiso::nrosy(V, F, b, bc, Eigen::VectorXi(), Eigen::VectorXd(), Eigen::MatrixXd(), degree, 0.4, X1, S);

        // Find the orthogonal vector
        Eigen::MatrixXd B1, B2, B3;
        igl::local_basis(V, F, B1, B2, B3);
        X2 = igl::rotate_vectors(X1, Eigen::VectorXd::Constant(1, igl::PI / 2), B1, B2);

        // output vector field and barycenters
        cgeomParseMatrixXd(X1, outX1Coords, outX1CoordsCount);
        cgeomParseMatrixXd(X2, outX2Coords, outX2CoordsCount);
        cgeomParseMatrixXd(S, outSingularities, outSingularitiesCount);
    }

    CGEOM_PARAM_API void cgeomNRosyFromFrameField(const int numVertices, const int numFaces, const int numConstraints, double *inCoords, int *inFaces, int *inConstrainedFaces, double *inX1, double *inX2, int degree,
                                    size_t *outX1CoordsCount, size_t *outX2CoordsCount, size_t *outSingularitiesCount, 
                                    double **outX1Coords, double **outX2Coords, double **outSingularities)
    {
        // Build mesh
        Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);
        Eigen::VectorXi b = Eigen::Map<Eigen::VectorXi>(inConstrainedFaces, numConstraints, 1);
        Eigen::MatrixXd bc1 = Eigen::Map<Eigen::MatrixXd>(inX1, numConstraints, 3);
        Eigen::MatrixXd bc2 = Eigen::Map<Eigen::MatrixXd>(inX2, numConstraints, 3);

        // Interpolate the frame field
        Eigen::MatrixXd FF1, FF2;
        igl::copyleft::comiso::frame_field(V, F, b, bc1, bc2, FF1, FF2);

        // Deform the mesh to transform the frame field in a cross field
        Eigen::MatrixXd V_deformed, FF1_deformed, FF2_deformed;
        igl::frame_field_deformer(V,F,FF1,FF2,V_deformed,FF1_deformed,FF2_deformed);

        // Compute face barycenters deformed mesh
        Eigen::MatrixXd B_deformed;
        igl::barycenter(V_deformed, F, B_deformed);

        // Find the closest crossfield to the deformed frame field
        Eigen::MatrixXd X1_deformed;
        igl::frame_to_cross_field(V,F,FF1_deformed,FF2_deformed,X1_deformed);

        // Find a smooth crossfield that interpolates the deformed constraints
        Eigen::MatrixXd bc_x(b.size(),3);
        for (unsigned i=0; i<b.size();++i)
        {
            bc_x.row(i) = X1_deformed.row(b(i));
        }

        // Create a smooth 4-RoSy field
        Eigen::VectorXd S;
        igl::copyleft::comiso::nrosy(V, F, b, bc_x, Eigen::VectorXi(), Eigen::VectorXd(), Eigen::MatrixXd(), degree, 0.4, X1_deformed, S);

        // Find the orthogonal vector
        Eigen::MatrixXd B1, B2, B3;
        igl::local_basis(V, F, B1, B2, B3);
        Eigen::MatrixXd X2_deformed = igl::rotate_vectors(X1_deformed, Eigen::VectorXd::Constant(1, igl::PI / 2), B1, B2);

        // output vector field and barycenters
        cgeomParseMatrixXd(X1_deformed, outX1Coords, outX1CoordsCount);
        cgeomParseMatrixXd(X2_deformed, outX2Coords, outX2CoordsCount);
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

    CGEOM_PARAM_API void cgeomPlanarization(const int inVertexCount, const int inQuadsCount, double *inCoords, int *inQuads, const int iterations, const double threshold, size_t *outVertexCount, size_t *outPlanarityCount, double **outCoords, double **outPlanarity)
    {
        Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords, inVertexCount, 3);
        Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inQuads, inQuadsCount, 4);

        Eigen::MatrixXd outV;
        igl::planarize_quad_mesh(V, F, iterations, threshold, outV);

        Eigen::VectorXd outP;
        igl::quad_planarity(V,F,outP);

        cgeomParseMatrixXd(outV, outCoords, outVertexCount);
        cgeomParseMatrixXd(outP, outPlanarity, outPlanarityCount);
    }

    CGEOM_PARAM_API void cgeomRotateVectors(const int numVectors, double *inX1Coords, double *inB1Coords, double *inB2Coords, double *inAngle, size_t *outCount, double **outX1Coords){
        Eigen::MatrixXd B1 = Eigen::Map<Eigen::MatrixXd>(inB1Coords,numVectors, 3);
        Eigen::MatrixXd B2 = Eigen::Map<Eigen::MatrixXd>(inB2Coords,numVectors, 3);
        Eigen::MatrixXd X1 = Eigen::Map<Eigen::MatrixXd>(inX1Coords,numVectors, 3);
        Eigen::VectorXd ang = Eigen::Map<Eigen::VectorXd>(inAngle, numVectors);

        Eigen::MatrixXd outX1 = igl::rotate_vectors(X1, ang, B1, B2);

        cgeomParseMatrixXd(outX1, outX1Coords, outCount);

        B1.setZero();
        B2.setZero();
        X1.setZero();
        ang.setZero();
        outX1.setZero();
    }

    CGEOM_PARAM_API int cgeomParallelTransport(const int numVertices, const int numFaces, double *inCoords, int *inFaces, const int inSourceVertex, const double initialPara, const double initialPerp, size_t *outCount, double **outCoords, double **outVecCoords, const char **errorMessage)
    {
        try{
            typedef Eigen::SparseMatrix<double> SparseMat;
            typedef Eigen::Matrix<double, 1, 1> Vector1d;
            typedef Eigen::Matrix<int, 1, 1> Vector1i;

            // Build mesh
            Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords, numVertices, 3);
            Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

            //Compute vector Laplacian and mass matrix
            Eigen::MatrixXi E, oE;//Compute Laplacian and mass matrix
            SparseMat vecL, vecM;
            igl::cr_vector_laplacian(V, F, E, oE, vecL);
            igl::cr_vector_mass(V, F, E, vecM);
            const int m = vecL.rows()/2; //The number of edges in the mesh

            //Convert the E / oE matrix format to list of edges / EMAP format required by the functions constructing scalar Crouzeix-Raviart functions
            Eigen::MatrixXi Elist(m,2), EMAP(3*F.rows(),1);
            for(int i=0; i<F.rows(); ++i) {
                for(int j=0; j<3; ++j) {
                const int e = E(i,j);
                EMAP(i+j*F.rows()) = e;
                if(oE(i,j)>0) {
                    Elist.row(e) << F(i, (j+1)%3), F(i, (j+2)%3);
                }
                }
            }
            SparseMat scalarL, scalarM;
            igl::crouzeix_raviart_massmatrix(V, F, Elist, EMAP, scalarM);
            igl::crouzeix_raviart_cotmatrix(V, F, Elist, EMAP, scalarL);

            //Compute edge midpoints & edge vectors
            Eigen::MatrixXd edgeMps, parVec, perpVec;
            igl::edge_midpoints(V, F, E, oE, edgeMps);
            igl::edge_vectors(V, F, E, oE, parVec, perpVec);

            //Perform the vector heat method
            // const double initialPara=0.95, initialPerp=0.08;
            const double t = 0.01;

            SparseMat Aeq;
            Eigen::VectorXd Beq;
            Eigen::VectorXi known = Eigen::Vector2i(inSourceVertex, inSourceVertex+m);
            Eigen::VectorXd knownVals = Eigen::Vector2d(initialPara, initialPerp);
            Eigen::VectorXd Y0 = Eigen::VectorXd::Zero(2*m), Yt;
            Y0(inSourceVertex) = initialPara; 
            Y0(inSourceVertex+m) = initialPerp;
            igl::min_quad_with_fixed(SparseMat(vecM+t*vecL), Eigen::VectorXd(-vecM*Y0), known, knownVals, Aeq, Beq, false, Yt);

            Eigen::VectorXd u0 = Eigen::VectorXd::Zero(m), ut;
            u0(inSourceVertex) = sqrt(initialPara*initialPara + initialPerp*initialPerp);
            Eigen::VectorXi knownScal = Vector1i(inSourceVertex);
            Eigen::VectorXd knownScalVals = Vector1d(u0(inSourceVertex));
            igl::min_quad_with_fixed(SparseMat(scalarM+t*scalarL), Eigen::VectorXd(-scalarM*u0), knownScal, knownScalVals, Aeq, Beq, false, ut);

            Eigen::VectorXd phi0 = Eigen::VectorXd::Zero(m), phit;
            phi0(inSourceVertex) = 1;
            Eigen::VectorXd knownScalValsPhi = Vector1d(1);
            igl::min_quad_with_fixed(SparseMat(scalarM+t*scalarL), Eigen::VectorXd(-scalarM*phi0), knownScal, knownScalValsPhi, Aeq, Beq, false, phit);

            Eigen::ArrayXd Xtfactor = ut.array() /
            (phit.array() * (Yt.array().segment(0,m)*Yt.array().segment(0,m)
                            + Yt.array().segment(m,m)*Yt.array().segment(m,m)).sqrt());
            Eigen::VectorXd Xt(2*m);
            Xt.segment(0,m) = Xtfactor * Yt.segment(0,m).array();
            Xt.segment(m,m) = Xtfactor * Yt.segment(m,m).array();

            //Compute scalar heat colors
            // igl::HeatGeodesicsData<double> hgData;
            // igl::heat_geodesics_precompute(V, F, hgData);
            // Eigen::VectorXd heatColor;
            // Eigen::VectorXi gamma = Elist.row(inSourceVertex);
            // igl::heat_geodesics_solve(hgData, gamma, heatColor);


            //Convert vector field for plotting
            Eigen::MatrixXd vecs(m, 3);
            for(int i=0; i<edgeMps.rows(); ++i) {
                vecs.row(i) = Xt(i)*parVec.row(i) + Xt(i+edgeMps.rows())*perpVec.row(i);
            }

            cgeomParseMatrixXd(vecs, outVecCoords, outCount);
            cgeomParseMatrixXd(edgeMps, outCoords, outCount);

            *errorMessage = "Success";
            return 0;
        }
        catch (const std::runtime_error &error)
        {
            *errorMessage = error.what();
            return 1;
        }
    }

    CGEOM_PARAM_API int cgeomEdgeVectors(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t *outCount, double **outEdgeMidCoords, double **outParCoords, double **outPerpCoords, const char **errorMessage){
        try{
            typedef Eigen::SparseMatrix<double> SparseMat;
            typedef Eigen::Matrix<double, 1, 1> Vector1d;
            typedef Eigen::Matrix<int, 1, 1> Vector1i;

            // Build mesh
            Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords, numVertices, 3);
            Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

            //Compute vector Laplacian and mass matrix
            Eigen::MatrixXi E, oE;//Compute Laplacian and mass matrix
            SparseMat vecL, vecM;
            igl::cr_vector_laplacian(V, F, E, oE, vecL);
            igl::cr_vector_mass(V, F, E, vecM);

            //Compute edge midpoints & edge vectors
            Eigen::MatrixXd edgeMps, parVec, perpVec;
            igl::edge_midpoints(V, F, E, oE, edgeMps);
            igl::edge_vectors(V, F, E, oE, parVec, perpVec);

            cgeomParseMatrixXd(edgeMps, outEdgeMidCoords, outCount);
            cgeomParseMatrixXd(parVec, outParCoords, outCount);
            cgeomParseMatrixXd(perpVec, outPerpCoords, outCount);

            *errorMessage = "Success";
            return 0;
        }
        catch (const std::runtime_error &error)
        {
            *errorMessage = error.what();
            return 1;
        }
    }

    CGEOM_PARAM_API int cgeomRemeshAlongIsoline(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double *inScalarField, double inIsoValue, size_t *outVertexCount, size_t *outFaceCount, double **outCoords, int **outFaces, const char **errorMessage){
        try{
            // Build mesh
            Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords, numVertices, 3);
            Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);
            Eigen::VectorXd S = Eigen::Map<Eigen::VectorXd>(inScalarField, numVertices, 1);

            // Output mesh
            Eigen::MatrixXd U;
            Eigen::MatrixXi G;
            Eigen::VectorXd SU;
            Eigen::VectorXd J;
            Eigen::SparseMatrix<double> BC;
            Eigen::VectorXi L;

            igl::remesh_along_isoline(V, F, S, inIsoValue, U, G, SU, J, BC, L);

            cgeomParseMatrixXd(U, outCoords, outVertexCount);
            cgeomParseMatrixXi(G, outFaces, outFaceCount);

            *errorMessage = "Success";
            return 0;
        }
        catch (const std::runtime_error &error)
        {
            *errorMessage = error.what();
            return 1;
        }
    }

    CGEOM_PARAM_API int cgeomHarmonicParametrization(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t *outUVCount, double **outUV, const char **errorMessage){
        try{
            // Build mesh
            Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords, numVertices, 3);
            Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3); 

            // Find the open boundary
            Eigen::VectorXi bnd;
            igl::boundary_loop(F,bnd);

            // Map the boundary to a circle, preserving edge proportions
            Eigen::MatrixXd bnd_uv;
            igl::map_vertices_to_circle(V,bnd,bnd_uv);

            // Harmonic parametrization for the internal vertices
            Eigen::MatrixXd V_uv;
            igl::harmonic(V,F,bnd,bnd_uv,1,V_uv);

            cgeomParseMatrixXd(V_uv, outUV, outUVCount);
            *errorMessage = "Success";
            return 0;
        }
        catch (const std::runtime_error &error)
        {
            *errorMessage = error.what();
            return 1;
        }
    }

    CGEOM_PARAM_API int cgeomLeastSquaresConformalMaps(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t *outUVCount, double **outUV, const char **errorMessage){
        try{
            // Build mesh
            Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords, numVertices, 3);
            Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3); 

            // Find the open boundary
            Eigen::VectorXi bnd;
            igl::boundary_loop(F,bnd);
            
            /////////////////////////////////////
            // TODO: fix boundary conditions. Exception is thrown!!!!!!!!!!
            // Fix two points on the boundary
            Eigen::VectorXi b = { bnd[0], bnd[2] }; //int(bnd[0]), bnd[int(bnd.size() / 2)] };

            Eigen::MatrixXd bc(2,3);
            bc << 0.0, 0.0, 0.0,
                  1.0, 0.0, 0.0;
            /////////////////////////////////////

            // LSCM parametrization
            Eigen::MatrixXd V_uv;
            igl::lscm(V,F,b,bc,V_uv);

            cgeomParseMatrixXd(V_uv, outUV, outUVCount);
            *errorMessage = "Success";
            return 0;
        }
        catch (const std::runtime_error &error)
        {
            *errorMessage = error.what();
            return 1;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////
    // libdirectional
    /////////////////////////////////////////////////////////////////////////////////////////
    CGEOM_PARAM_API int cgeomRemeshAlignedToCurvatureField(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double edgeLength, size_t *outVertexCount, size_t *outFaceCount, size_t *outDegreesCount, double **outCoords, int **outFaces, int **outDegrees, const char **errorMessage)
    {
        try
        {
            // =========================================================================
            ////////////////// Step 1: Build the TriMesh and compute the face tangent bundle
            // =========================================================================
            // Build mesh
            Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
            Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);
            int degree = 6;

            // Normalize V to unit bounding box diagonal before all processing.
            // This makes avgEdgeLength, bboxNorm, and bboxDiag all predictable
            // (bboxDiag == 1 exactly after scaling), eliminating scale-dependent
            // discrepancies between mm/cm/m Rhino meshes.  edgeLength is scaled by
            // the same factor so it remains expressed in the same relative units.
            // outV is unscaled at the end before returning.
            const double meshScale = igl::bounding_box_diagonal(V);
            V.array() /= meshScale;
            const double scaledEdgeLength = edgeLength / meshScale;

            directional::TriMesh mesh;
            mesh.set_mesh(V, F);

            // Clamp edgeLength from below so the mesher never sees more than
            // kMaxIsolinesPerEdge iso-lines per triangle edge, which causes SIGABRT
            // in the exact-arithmetic code.  No upper clamp: large edgeLength simply
            // produces fewer iso-lines per triangle, which the mesher handles fine.
            const double kMaxIsolinesPerEdge = 2.0;
            const double minEdgeLength = mesh.avgEdgeLength / kMaxIsolinesPerEdge;
            const double effectiveEdgeLength = std::max(scaledEdgeLength, minEdgeLength);

            directional::PCFaceTangentBundle ftb;
            ftb.init(mesh);

            // =========================================================================
            ////////////////// Step 2: Compute curvature-aligned constraints
            // =========================================================================
            // Build a smooth 6-RoSy field aligned to principal curvatures.
            //
            // A piecewise-constant per-face raw field (built directly from curvature directions)
            // causes the exact-arithmetic isoline mesher to crash: the constant gradient within
            // each face inevitably aligns exactly with some mesh edge ("line triangle overlap").
            // Instead, use power_field() to solve for a globally smooth field soft-constrained
            // to the curvature directions, then convert to raw.  A smooth field's isolines are
            // in general position w.r.t. mesh edges, as the pre-optimised .rawfield files were.
                
            // Curvature below this threshold (≈ 1/10 of the scale set by avg edge length) is
            // treated as flat.  Adjust the denominator if your mesh has very fine flat regions.
            const double flatThreshold = 1.0 / (10.0 * mesh.avgEdgeLength);
            
            int numFaces = mesh.F.rows();
            Eigen::VectorXi constSpaces(numFaces);
            Eigen::MatrixXd constVectors(numFaces, 3);   // one representative 3D vector per face
            Eigen::VectorXd alignWeights(numFaces);

            for (int f = 0; f < numFaces; f++) {
                constSpaces(f) = f;

                // Average vertex min-curvature directions to the face, resolving sign ambiguity.
                Eigen::Vector3d ref = mesh.minVertexPrincipalDirections.row(mesh.F(f, 0));
                Eigen::Vector3d d1sum = Eigen::Vector3d::Zero();
                double kMaxFace = 0.0;
                for (int j = 0; j < 3; j++) {
                    int v = mesh.F(f, j);
                    kMaxFace += std::max(std::abs(mesh.vertexPrincipalCurvatures(v, 0)),
                                        std::abs(mesh.vertexPrincipalCurvatures(v, 1)));
                    Eigen::Vector3d dir = mesh.minVertexPrincipalDirections.row(v);
                    if (dir.dot(ref) < 0) dir = -dir;
                    d1sum += dir;
                }
                kMaxFace /= 3.0;

                Eigen::Vector3d n = mesh.faceNormals.row(f).normalized();
                Eigen::Vector3d d1;
                if (kMaxFace < flatThreshold || d1sum.norm() < 1e-10) {
                    d1 = mesh.FBx.row(f).normalized();
                    alignWeights(f) = 0.0;   // flat patch: no alignment preference
                } else {
                    d1 = d1sum - d1sum.dot(n) * n;
                    if (d1.norm() < 1e-10) {
                        d1 = mesh.FBx.row(f).normalized();
                        alignWeights(f) = 0.0;
                    } else {
                        d1.normalize();
                        alignWeights(f) = kMaxFace;  // weight proportional to curvature magnitude
                    }
                }
                constVectors.row(f) = d1.transpose();
            }

            // =========================================================================
            ////////////////// Step 3: Smooth power field optimization with curvature alignment soft constraints
            // =========================================================================
        
            // Count zero-weight faces for diagnostics (reported in error message).
            int zeroWeightFaces = (alignWeights.array() == 0.0).cast<int>().sum();

            // Solve for a smooth 6-RoSy power field with per-face curvature soft constraints.
            directional::CartesianField powerField;
            directional::power_field(ftb, constSpaces, constVectors, alignWeights, degree, powerField);
            
            // =========================================================================
            ////////////////// Step 4: Convert the power field to a raw field (6 explicit vectors per face), normalizing the vectors to unit length
            // =========================================================================
            directional::CartesianField rawField;
            // Extract the 6 CCW-ordered raw vectors per face from the power field.
            directional::power_to_raw(powerField, degree, rawField, /*normalize=*/true);

            // Sanitize: replace any zero or NaN vectors with the face tangent basis
            // vector FBx.  Zero vectors occur on flat regions where alignWeights=0 and
            // the power field solver returns a near-zero solution.  They cause division
            // by zero in principal_matching (freeCoeff *= vecjgc/transvecjfc) which
            // propagates NaN into effort_to_indices, triggering its assert.
            for (int f = 0; f < mesh.F.rows(); f++) {
                for (int j = 0; j < degree; j++) {
                    Eigen::RowVector2d v = rawField.intField.block(f, 2*j, 1, 2);
                    if (!v.allFinite() || v.norm() < 1e-10) {
                        // Project FBx into the face's 2D tangent frame (it is already the x-axis by construction)
                        rawField.intField.block(f, 2*j, 1, 2) << 1.0, 0.0;
                    }
                }
            }
            // Re-sync the extrinsic field from the corrected intrinsic one
            rawField.set_intrinsic_field(rawField.intField);

            // =========================================================================
            ////////////////// Step 5: Principal matching — find which vectors correspond across edges
            // =========================================================================
            // Guard with SIGABRT handler: effort_to_indices inside principal_matching
            // asserts that indices are integer — can still fire for numerically marginal fields.
            {
                auto* pmHandler = signal(SIGABRT, mesherAbortHandler);
                s_mesherActive = 1;
                if (sigsetjmp(s_mesherJmpBuf, 1) != 0) {
                    s_mesherActive = 0;
                    signal(SIGABRT, pmHandler);
                    throw std::runtime_error(
                        "principal_matching failed (effort_to_indices: non-integer index). "
                        "The power field has a degenerate singularity configuration on this mesh.");
                }
                directional::principal_matching(rawField);
                s_mesherActive = 0;
                signal(SIGABRT, pmHandler);
            }

            // =========================================================================
            ////////////////// Step 6: Integrate the field to get a seamless parameterization, cutting the mesh along seams where the field is discontinuous and preparing the linear system for integer-alignment of the seam jumps
            // =========================================================================
            // This solves for N scalar functions whose gradients match the field, with integer-seamless conditions across the seam curves.
            directional::IntegrationData intData(degree);
            
            directional::TriMesh meshCut;
            directional::CartesianField combedField;
            // For degree=6, override the default sign symmetry (n=3) with triangular symmetry
            // (n=2), which is the correct 2-parameter reduction for hex/triangle meshing.
            // This must be set before setup_integration, which uses intData.n and linRed.
            intData.set_triangular_symmetry(degree);

            // Install SIGABRT guard before setup_integration: its halfedge-traversal
            // loop (setup_integration.h:315) can run forever if the singularity/cut
            // graph has a cycle with all interior valence-2 vertices.  The guard also
            // covers the integrate→mesher retry loop below.
            auto* prevHandler = signal(SIGABRT, mesherAbortHandler);
            s_mesherActive = 1;
            if (sigsetjmp(s_mesherJmpBuf, 1) != 0) {
                s_mesherActive = 0;
                signal(SIGABRT, prevHandler);
                throw std::runtime_error(
                    "setup_integration failed (SIGABRT). "
                    "The field singularity configuration is degenerate on this mesh.");
            }
            directional::setup_integration(rawField, intData, meshCut, combedField);
            s_mesherActive = 0;

            intData.verbose=false;
            // integralSeamless must be true: the mesher requires seam-jump variables
            // to be rounded to integers so that iso-lines close up across cut edges.
            // With false, the parameterization is smooth but the mesher produces
            // garbage or crashes.  The parameter accepted by this function is unused.
            intData.integralSeamless=true;
            intData.roundSeams=false;
            // Without singularities, vertex 0 is the integration anchor and is fixed to
            // function value 0 (an integer).  An isoline at value 0 then coincides with a
            // triangle edge → "line triangle overlap" crash in the mesher.
            // Shifting the anchor to 0.5 moves all isolines to half-integers so they always
            // cross triangle interiors, never edges.
            intData.fixedValues.resize(intData.n);
            intData.fixedValues.setConstant(0.5);

            // The integrator computes paramLength = bboxNorm * lengthRatio, which sets
            // the 3D spacing between iso-lines.  bboxDiag is invariant to upsampling
            // (midpoint subdivision never moves vertices) so it stays consistent
            // regardless of how many upsample iterations were performed above.
            // avgEdgeLength must NOT be used here: it halves with each subdivision,
            double bboxDiag = igl::bounding_box_diagonal(V);
            intData.lengthRatio = effectiveEdgeLength / bboxDiag;
        
            // =========================================================================
            ////////////////// Steps 7–8: Integrate and mesh, retrying with different
            // anchor offsets if the exact-arithmetic mesher hits a degenerate
            // configuration.
            //
            // fixedValues shifts all iso-lines by a constant.  Different offsets
            // avoid different degeneracies (iso-line on an edge, parallel pencils,
            // zero pVec scale) without changing the topology of the output mesh.
            // Irrational offsets are tried in order; the first that succeeds is used.
            // Simple rationals like 1/10, 1/5 keep all intermediate products small.
            // They may coincide with an iso-line on an edge for some meshes, 
            //but the retry loop will skip them if they fail. The first that succeeds is used.
            static const double kAnchorCandidates[] = { 0.1, 0.2, 0.3, 0.4, 0.6, 0.7, 0.8, 0.9 };
            static const int kNumCandidates = sizeof(kAnchorCandidates) / sizeof(kAnchorCandidates[0]);

            Eigen::MatrixXd outV;
            Eigen::VectorXi outD;
            Eigen::MatrixXi outF;
            bool mesherOk = false;
            int successfulAttempt = -1;

            // prevHandler was already set when we installed the guard before setup_integration.
            // Re-use the same handler for the integrate→mesher retry loop.
            for (int attempt = 0; attempt < kNumCandidates && !mesherOk; ++attempt) {
                intData.fixedValues.setConstant(kAnchorCandidates[attempt]);

                s_mesherActive = 1;
                if (sigsetjmp(s_mesherJmpBuf, /*savemask=*/1) != 0) {
                    // SIGABRT caught anywhere in this attempt — try next anchor.
                    s_mesherActive = 0;
                    continue;
                }

                Eigen::MatrixXd NFunction, NCornerFunction;
                directional::integrate(combedField, intData, meshCut, NFunction, NCornerFunction);

                // Guard against NaN in the parameterization.
                // ENumber_internal.h uses while(true) to find a continued-fraction
                // approximation; if x==NaN then (|approx-x|<=tol) is always false
                // and the loop never terminates — freezing Rhino.
                // NaN appears when integrate() receives a degenerate combed field
                // (e.g. all-umbilic sphere where curvature directions are undefined).
                if (!NFunction.allFinite() || !NCornerFunction.allFinite()) {
                    s_mesherActive = 0;
                    continue;   // try next anchor; all will fail → error below
                }

                directional::MesherData mData;
                directional::setup_mesher(meshCut, intData, mData);

                mesherOk = directional::mesher(mesh, mData, outV, outD, outF);
                s_mesherActive = 0;
                if (mesherOk) successfulAttempt = attempt;
            }

            signal(SIGABRT, prevHandler);

            if (!mesherOk) {
                std::ostringstream msg;
                msg << "\nRemesh Failed\n"
                    << "  Input mesh\n"
                    << "    vertices       : " << mesh.V.rows() << "\n"
                    << "    faces          : " << mesh.F.rows() << "\n"
                    << "    avgEdgeLength  : " << (mesh.avgEdgeLength * meshScale) << "\n"
                    << "    bboxDiagonal   : " << meshScale << "\n"
                    << "  Parameters\n"
                    << "    edgeLength (in): " << edgeLength << "\n"
                    << "    edgeLength (eff): " << (effectiveEdgeLength * meshScale)
                    << (scaledEdgeLength < minEdgeLength ? " (clamped from " + std::to_string(edgeLength) + ")" : "") << "\n"
                    << "    anchors tried  : " << kNumCandidates << "\n"
                    << "  Diagnosis\n"
                    << "    iso-lines/edge : " << (mesh.avgEdgeLength / effectiveEdgeLength) << "\n"
                    << "    min edgeLength : " << (minEdgeLength * meshScale) << "\n"
                    << "    zero-weight faces: " << zeroWeightFaces << " / " << mesh.F.rows()
                    << " (" << (100.0 * zeroWeightFaces / mesh.F.rows()) << "%)\n"
                    << (zeroWeightFaces == mesh.F.rows()
                        ? "  Note: ALL faces have zero curvature-alignment weight.\n"
                          "    This mesh is isotropic (e.g. a sphere or flat plane) — principal\n"
                          "    curvature directions are undefined everywhere.  The resulting field\n"
                          "    is arbitrary and may produce NaN in the parameterization.\n"
                          "    Curvature-aligned remeshing is not meaningful on this shape.\n"
                        : "")
                    << "  Fix\n"
                    << "    For a denser mesh : provide an input mesh with avgEdgeLength <= "
                    << (edgeLength / kMaxIsolinesPerEdge) << "\n"
                    << "    For a coarser mesh: increase edgeLength (no input mesh change needed).\n"
                    << "    Safe edgeLength range for this input mesh: ["
                    << (minEdgeLength * meshScale) << ", inf)\n";
                throw std::runtime_error(msg.str());
            }

            // =========================================================================
            ////////////////// Step 9: Output
            // =========================================================================
            outV.array() *= meshScale;   // restore original units
            cgeomParseMatrixXd(outV, outCoords, outVertexCount);
            cgeomParseMatrixXi(outF, outFaces, outFaceCount);
            cgeomParseVectorXi(outD, outDegrees, outDegreesCount);

            std::ostringstream log;
            log << "\nRemesh Success\n"
                << "  Input mesh\n"
                << "    vertices       : " << mesh.V.rows() << "\n"
                << "    faces          : " << mesh.F.rows() << "\n"
                << "    avgEdgeLength  : " << (mesh.avgEdgeLength * meshScale) << "\n"
                << "    bboxDiagonal   : " << meshScale << "\n"
                << "  Parameters\n"
                << "    edgeLength (in): " << edgeLength << "\n"
                << "    edgeLength (eff): " << (effectiveEdgeLength * meshScale)
                << (scaledEdgeLength < minEdgeLength ? " (clamped from " + std::to_string(edgeLength) + ")" : "") << "\n"
                << "    anchor offset  : " << kAnchorCandidates[successfulAttempt]
                << " (attempt " << (successfulAttempt + 1) << "/" << kNumCandidates << ")\n"
                << "    iso-lines/edge : " << (mesh.avgEdgeLength / effectiveEdgeLength) << "\n"
                << "  Output mesh\n"
                << "    vertices       : " << outV.rows() << "\n"
                << "    faces          : " << outF.rows() << "\n";
            *errorMessage = strdup(log.str().c_str());
            return 0;
        }
        catch (const std::runtime_error &error)
        {
            std::string msg = error.what();
            char* buffer = (char*)std::malloc(msg.size() + 1);
            std::memcpy(buffer, msg.c_str(), msg.size() + 1);
            *errorMessage = buffer;
            return 1;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////
    // Instant Meshes
    /////////////////////////////////////////////////////////////////////////////////////////
    CGEOM_PARAM_API int cgeomInstantMeshesRemesh(
        const int numVertices, const int numFaces,
        double* inCoords, int* inFaces,
        double edgeLength, int targetVertexCount,
        size_t* outVertexCount, size_t* outFaceCount, size_t* outDegreesCount,
        double** outCoords, int** outFaces, int** outDegrees,
        const char** errorMessage)
    {
        try
        {
            std::vector<float> verts;
            std::vector<int>   faceIndices;
            std::vector<int>   faceDegrees;

            // rosy=6, posy=3: triangle-aligned output.
            // targetVertexCount>0 overrides edgeLength; edgeLength<=0 triggers auto default.
            float actualScale = 0.f;
            std::string err = instantMeshesProcess(
                inCoords, numVertices,
                inFaces,  numFaces,
                6, 3, (float)edgeLength, targetVertexCount,
                verts, faceIndices, faceDegrees, actualScale);

            if (!err.empty())
                throw std::runtime_error(err);

            int nVout = (int)verts.size() / 3;
            int nFout = (int)faceIndices.size()/3;

            // outCoords: nVout x 3 column-major (Eigen MatrixXd layout)
            Eigen::MatrixXd outV(nVout, 3);
            for (int i = 0; i < nVout; ++i) {
                outV(i, 0) = (double)verts[3*i + 0];
                outV(i, 1) = (double)verts[3*i + 1];
                outV(i, 2) = (double)verts[3*i + 2];
            }
            cgeomParseMatrixXd(outV, outCoords, outVertexCount);

            // outFaces: nVout x 3 column-major (Eigen MatrixXd layout)
            Eigen::MatrixXi outF(nFout, 3);
            for (int i = 0; i < nFout; ++i) {
                outF(i, 0) = (int)faceIndices[3*i + 0];
                outF(i, 1) = (int)faceIndices[3*i + 1];
                outF(i, 2) = (int)faceIndices[3*i + 2];
            }
            cgeomParseMatrixXi(outF, outFaces, outFaceCount);

            std::ostringstream log;
            log << "\n[cgeomInstantMeshesRemesh] Success\n"
                << "  Input  : " << numVertices << " vertices, " << numFaces << " faces\n"
                << "  Output : " << nVout << " vertices, " << nFout << " faces\n"
                << (targetVertexCount > 0
                    ? "  targetVertexCount: " + std::to_string(targetVertexCount) + "\n"
                    : "  edgeLength (requested): " + std::to_string(edgeLength) + "\n")
                << "  edgeLength (used): " << actualScale << "\n"
                << ((float)edgeLength > 0.f && actualScale > (float)edgeLength * 1.001f
                    ? "  WARNING: edgeLength was clamped up to prevent >500k vertices\n"
                    : "");
            *errorMessage = strdup(log.str().c_str());
            return 0;
        }
        catch (const std::exception& e)
        {
            std::string msg = e.what();
            char* buffer = (char*)std::malloc(msg.size() + 1);
            std::memcpy(buffer, msg.c_str(), msg.size() + 1);
            *errorMessage = buffer;
            return 1;
        }
    }
}