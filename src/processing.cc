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


extern "C"
{
#include "processing.h"
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

    CGEOM_PARAM_API void cgeomParseStdVectorInt(const std::vector<int> m, int **outData, size_t *outCount){
        *outCount = m.size();
        auto sF = *outCount * sizeof(int);
        *outData = static_cast<int *>(malloc(sF));
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
            igl::cr_vector_mass(V, F, E, oE, vecM);
            igl::cr_vector_laplacian(V, F, E, oE, vecL);
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
            igl::cr_vector_mass(V, F, E, oE, vecM);
            igl::cr_vector_laplacian(V, F, E, oE, vecL);

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

            // Parse quad mesh vertex data
            *outVertexCount = U.size();
            auto sV = *outVertexCount * sizeof(double);
            *outCoords = static_cast<double *>(malloc(sV));

            std::vector<double> coords;
            for(int i=0; i<U.rows(); i++){
                auto vertex = U.row(i);
                coords.push_back(vertex[0]);
                coords.push_back(vertex[1]);
                coords.push_back(vertex[2]);
            }
            std::memcpy(*outCoords, coords.data(), sV);

            // Parse quad mesh face data
            *outFaceCount = G.size();
            auto sF = *outFaceCount * sizeof(int);
            *outFaces = static_cast<int *>(malloc(sF));

            std::vector<int> faces;
            for(int i=0; i<G.rows(); i++){
                auto f = G.row(i);
                faces.push_back(f[0]);
                faces.push_back(f[1]);
                faces.push_back(f[2]);
            }
            std::memcpy(*outFaces, faces.data(), sF);

            *errorMessage = "Success";
            return 0;
        }
        catch (const std::runtime_error &error)
        {
            *errorMessage = error.what();
            return 1;
        }
    }
}