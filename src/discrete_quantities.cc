#include <igl/principal_curvature.h>
#include <igl/per_vertex_normals.h>
#include <igl/per_face_normals.h>
#include <igl/per_corner_normals.h>
#include <igl/gaussian_curvature.h>
#include <igl/massmatrix.h>
#include <igl/invert_diag.h>
#include <igl/cotmatrix.h>
#include <igl/barycenter.h>
#include <igl/doublearea.h>
#include <igl/grad.h>
#include <igl/rotate_vectors.h>
#include <igl/boundary_facets.h>
#include <igl/unique.h>
#include <igl/setdiff.h>
#include <igl/slice_into.h>
#include <igl/min_quad_with_fixed.h>

extern "C"
{
#include "discrete_quantities.h"
#include "parameterizations.h"
}

namespace CGeom
{
    ///////////////////////////////////////////////
    // Discrete Geometric Quantities And Operators
    ///////////////////////////////////////////////

    CGEOM_QUANT_API void cgeomPerVertexAsymptoticDirections(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t *outX1Count, size_t *outX2Count, double **outX1, double **outX2){ 
        // Build mesh
        Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        // Compute curvature directions via quadric fitting
        Eigen::MatrixXd dir1,dir2;
        Eigen::VectorXd val1,val2;
        igl::principal_curvature(V,F,dir1,dir2,val1,val2);

        // Compute asymptotic directions
        Eigen::MatrixXd adir1(numVertices, 3);
        Eigen::MatrixXd adir2(numVertices, 3);
        for(int i=0; i<numVertices; i++){
            const auto k1 = val1(i);
            const auto k2 = val2(i);

            // Check for anticlastic surface-regions
            if (k1 * k2 > 0){
                adir1.row(i) = Eigen::Vector3d(0,0,0);
                adir2.row(i) = Eigen::Vector3d(0,0,0);
            }else{
                Eigen::VectorXd theta(1);
                theta(0) = atan2(-k1,k2);

                Eigen::MatrixXd a1(1,3);
                a1.row(0) = dir1.row(i);
                Eigen::MatrixXd a2(1,3);
                a2.row(0) = dir2.row(i);
                adir1.row(i) = igl::rotate_vectors(a1, theta, dir1.row(i), dir2.row(i));
                adir2.row(i) = igl::rotate_vectors(a2, theta, dir1.row(i), dir2.row(i));
            }
        }

        // Parse data
        cgeomParseMatrixXd(adir1, outX1, outX1Count);
        cgeomParseMatrixXd(adir2, outX2, outX2Count);

        // Release Eigen Matrix Memory
        F.setZero();
        V.setZero();
        adir1.setZero();
        adir2.setZero();
        dir1.setZero();
        dir2.setZero();
        val1.setZero();
        val2.setZero();
    }

    CGEOM_QUANT_API void cgeomPerFaceAsymptoticDirections(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t *outX1Count, size_t *outX2Count, double **outX1, double **outX2){ 
        // Build mesh
        Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        // Compute curvature directions via quadric fitting
        Eigen::MatrixXd dir1,dir2;
        Eigen::VectorXd val1,val2;
        igl::principal_curvature(V,F,dir1,dir2,val1,val2);

        // Compute per vertex asymptotic directions
        Eigen::MatrixXd adir1(numVertices, 3);
        Eigen::MatrixXd adir2(numVertices, 3);
        for(int i=0; i<numVertices; i++){
            const auto k1 = val1(i);
            const auto k2 = val2(i);

            // Check for anticlastic surface-regions
            if (k1 * k2 > 0){
                adir1.row(i) = Eigen::Vector3d(0,0,0);
                adir2.row(i) = Eigen::Vector3d(0,0,0);
            }else{
                Eigen::VectorXd theta(1);
                theta(0) = atan2(-k1,k2);

                Eigen::MatrixXd a1(1,3);
                a1.row(0) = dir1.row(i);
                Eigen::MatrixXd a2(1,3);
                a2.row(0) = dir2.row(i);
                adir1.row(i) = igl::rotate_vectors(a1, theta, dir1.row(i), dir2.row(i));
                adir2.row(i) = igl::rotate_vectors(a2, theta, dir1.row(i), dir2.row(i));
            }
        }

        // Compute per face directions as the average of vertex directions
        Eigen::MatrixXd fdir1(numFaces, 3);
        Eigen::MatrixXd fdir2(numFaces, 3);
        for(int i=0; i<numFaces; i++){
            Eigen::Vector3d a1(0,0,0);
            Eigen::Vector3d a2(0,0,0);
            for(int j=0; j<3; j++){
                a1 += adir1.row(F.coeff(i,j));
                a2 += adir2.row(F.coeff(i,j));
            }
            a1 /= 3;
            a2 /= 3;

            fdir1.row(i) = a1;
            fdir2.row(i) = a2;
        }

        // Parse data
        cgeomParseMatrixXd(fdir1, outX1, outX1Count);
        cgeomParseMatrixXd(fdir2, outX2, outX2Count);

        // Release Eigen Matrix Memory
        F.setZero();
        V.setZero();
        adir1.setZero();
        adir2.setZero();
        fdir1.setZero();
        fdir2.setZero();
        dir1.setZero();
        dir2.setZero();
        val1.setZero();
        val2.setZero();
    }

    CGEOM_QUANT_API void cgeomPerVertexPrincipalCurvatures(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t *outX1CoordsCount, size_t *outX2CoordsCount, size_t *outK1Count, size_t *outK2Count, double **outX1Coords, double **outX2Coords, double **outK1, double **outK2)
    {
        // Build mesh
        Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        // Compute curvature directions via quadric fitting
        Eigen::MatrixXd x1,x2;
        Eigen::VectorXd k1,k2;
        igl::principal_curvature(V,F,x1,x2,k1,k2);

        // Parse data
        cgeomParseMatrixXd(x1, outX1Coords, outX1CoordsCount);
        cgeomParseMatrixXd(x2, outX2Coords, outX2CoordsCount);
        cgeomParseMatrixXd(k1, outK1, outK1Count);
        cgeomParseMatrixXd(k2, outK2, outK2Count);

        // Release Eigen Matrix Memory
        V.setZero();
        F.setZero();
        x1.setZero();
        x2.setZero();
        k1.setZero();
        k2.setZero();
    }

    CGEOM_QUANT_API void cgeomPerFacePrincipalCurvatures(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t *outX1CoordsCount, size_t *outX2CoordsCount, size_t *outK1Count, size_t *outK2Count, double **outX1Coords, double **outX2Coords, double **outK1, double **outK2)
    {
        // Build mesh
        Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        // Compute per vertex curvature directions via quadric fitting
        Eigen::MatrixXd vX1,vX2;
        Eigen::VectorXd vK1,vK2;
        igl::principal_curvature(V,F,vX1,vX2,vK1,vK2);

        // Compute per face curvature directions as the average of vertex values
        Eigen::MatrixXd fX1(numFaces, 3);
        Eigen::MatrixXd fX2(numFaces, 3);
        Eigen::VectorXd fK1(numFaces);
        Eigen::VectorXd fK2(numFaces);
        for(int i=0; i<numFaces; i++){
            Eigen::Vector3d x1(0,0,0);
            Eigen::Vector3d x2(0,0,0);
            double k1 = 0;
            double k2 = 0;
            for(int j=0; j<3; j++){
                x1 += vX1.row(F.coeff(i,j));
                x2 += vX2.row(F.coeff(i,j));
                k1 += vK1(F.coeff(i,j));
                k2 += vK2(F.coeff(i,j));
            }
            x1 /= 3;
            x2 /= 3;
            k1 /= 3;
            k2 /= 3;

            fX1.row(i) = x1;
            fX2.row(i) = x2;
            fK1(i) = k1;
            fK2(i) = k2;
        }

        // Parse data
        cgeomParseMatrixXd(fX1, outX1Coords, outX1CoordsCount);
        cgeomParseMatrixXd(fX2, outX2Coords, outX2CoordsCount);
        cgeomParseMatrixXd(fK1, outK1, outK1Count);
        cgeomParseMatrixXd(fK2, outK2, outK2Count);

        // Release Eigen Matrix Memory
        V.setZero();
        F.setZero();
        vX1.setZero();
        vX2.setZero();
        vK1.setZero();
        vK2.setZero();
        fX1.setZero();
        fX2.setZero();
        fK1.setZero();
        fK2.setZero();
    }

    CGEOM_QUANT_API void cgeomNormalsPerVertex(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outNorm){
        // Build mesh
        Eigen::MatrixXd V;
        Eigen::MatrixXi F;
        V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        // Compute per-vertex normals
        Eigen::MatrixXd N;
        igl::per_vertex_normals(V,F,N);

        // Parse data
        auto sizeVectors = N.size() * sizeof(double);
        *outNorm = static_cast<double *>(malloc(sizeVectors));

        std::vector<double> norm;
        for (int i = 0; i < N.rows(); i++)
        {
            norm.push_back(N(i,0));
            norm.push_back(N(i,1));
            norm.push_back(N(i,2));
        }
        std::memcpy(*outNorm, norm.data(), sizeVectors);

        // Release Eigen Matrix Memory
        V.setZero();
        F.setZero();
        N.setZero();
    }
    
    CGEOM_QUANT_API void cgeomNormalsPerFace(const int numVertices, const int numFaces, double *inCoords, int *inFaces,  double **outNorm){
        // Build mesh
        Eigen::MatrixXd V;
        Eigen::MatrixXi F;
        V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        // Compute per-face normals
        Eigen::MatrixXd N;
        igl::per_face_normals(V,F,N);

        // Parse data
        auto sizeVectors = N.size() * sizeof(double);
        *outNorm = static_cast<double *>(malloc(sizeVectors));

        std::vector<double> norm;
        for (int i = 0; i < N.rows(); i++)
        {
            norm.push_back(N(i,0));
            norm.push_back(N(i,1));
            norm.push_back(N(i,2));
        }
        std::memcpy(*outNorm, norm.data(), sizeVectors);

        // Release Eigen Matrix Memory
        V.setZero();
        F.setZero();
        N.setZero();
    }
    
    CGEOM_QUANT_API void cgeomNormalsPerCorner(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double angle,  double **outNorm){
        // Build mesh
        Eigen::MatrixXd V;
        Eigen::MatrixXi F;
        V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        // Compute per-corner normals, |dihedral angle| > angle degrees --> crease
        Eigen::MatrixXd N;
        igl::per_corner_normals(V,F,angle,N);

        // Parse data
        auto sizeVectors = N.size() * sizeof(double);
        *outNorm = static_cast<double *>(malloc(sizeVectors));

        std::vector<double> norm;
        for (int i = 0; i < N.rows(); i++)
        {
            norm.push_back(N(i,0));
            norm.push_back(N(i,1));
            norm.push_back(N(i,2));
        }
        std::memcpy(*outNorm, norm.data(), sizeVectors);

        // Release Eigen Matrix Memory
        V.setZero();
        F.setZero();
        N.setZero();
    }

    CGEOM_QUANT_API void cgeomGaussianCurvature(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outK){
        // Build mesh
        Eigen::MatrixXd V;
        Eigen::MatrixXi F;
        V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        Eigen::VectorXd K;
        // Compute integral of Gaussian curvature
        igl::gaussian_curvature(V,F,K);
        // Compute mass matrix
        Eigen::SparseMatrix<double> M,Minv;
        igl::massmatrix(V,F,igl::MASSMATRIX_TYPE_DEFAULT,M);
        igl::invert_diag(M,Minv);
        // Divide by area to get integral average
        K = (Minv*K).eval();

        // Parse data
        auto sizeVectors = K.size() * sizeof(double);
        *outK = static_cast<double *>(malloc(sizeVectors));

        std::vector<double> k;
        for (int i = 0; i < K.size(); i++)
        {
            k.push_back(K(i));
        }
        std::memcpy(*outK, k.data(), sizeVectors);

        // Release Eigen Matrix Memory
        V.setZero();
        F.setZero();
        K.setZero();
        M.setZero();
        Minv.setZero();
    }

    CGEOM_QUANT_API void cgeomMeanCurvature(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outH){
        // Build mesh
        Eigen::MatrixXd V;
        Eigen::MatrixXi F;
        V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);


        // Alternative discrete mean curvature
        Eigen::MatrixXd HN;
        Eigen::SparseMatrix<double> L,M,Minv;
        igl::cotmatrix(V,F,L);
        igl::massmatrix(V,F,igl::MASSMATRIX_TYPE_VORONOI,M);
        igl::invert_diag(M,Minv);
        // Laplace-Beltrami of position
        HN = -Minv*(L*V);
        // Extract magnitude as mean curvature
        Eigen::VectorXd H = HN.rowwise().norm();

        // Parse data
        auto sizeVectors = H.size() * sizeof(double);
        *outH = static_cast<double *>(malloc(sizeVectors));

        std::vector<double> h;
        for(int i = 0; i < H.size(); i++)
        {
            h.push_back(H(i));
        }
        std::memcpy(*outH, H.data(), sizeVectors);

        // Release Eigen Matrix Memory
        V.setZero();
        F.setZero();
        H.setZero();
        HN.setZero();
        L.setZero();
        M.setZero();
        Minv.setZero();
    }

    CGEOM_QUANT_API void cgeomLaplacianSmoothingForOpenMesh(const int numVertices, const int numFaces, const int numBoundaries, double *inCoords, int *inFaces, int *inBoundaries, int *inInteriors, int numIterations, size_t * outCoordsCount, double **outCoords)
    {
        // Build mesh
        Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords, numVertices, 3);
        Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        Eigen::VectorXi interior_vertices = Eigen::Map<Eigen::VectorXi>(inInteriors, numVertices - numBoundaries, 1);
        Eigen::VectorXi boundary_vertices = Eigen::Map<Eigen::VectorXi>(inBoundaries, numBoundaries, 1);

        Eigen::MatrixXd A(numBoundaries, 3);
        for (int i = 0; i < numBoundaries; i++)
            A.row(i) = V.row(boundary_vertices(i));

        for (int i = 0; i < numIterations; i++)
        {
            // Construct and slice up Laplacian
            Eigen::SparseMatrix<double> laplacian, laplacian_interior, laplacian_boundary;
            igl::cotmatrix(V, F, laplacian);
            igl::slice(laplacian, interior_vertices, interior_vertices, laplacian_interior);
            igl::slice(laplacian, interior_vertices, boundary_vertices, laplacian_boundary);

            Eigen::SimplicialLLT<Eigen::SparseMatrix<double>> solver(-laplacian_interior);
            const auto U = solver.solve(laplacian_boundary * A).eval();

            // slice into solution
            igl::slice_into(U, interior_vertices, Eigen::Vector3i(0, 1, 2), V);
        }

        // Parse data
        cgeomParseMatrixXd(V, outCoords, outCoordsCount);

        // Release Eigen Matrix Memory
        F.setZero();
        V.setZero();
    }

    CGEOM_QUANT_API void cgeomLaplacianSmoothingForCloseMesh(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double smoothing, int numIterations, size_t * outCoordsCount, double **outCoords)
    {
        // Build mesh
        Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords, numVertices, 3);
        Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        Eigen::SparseMatrix<double> L;
        igl::cotmatrix(V, F, L);

        for (int i = 0; i < numIterations; i++)
        {
            // Recompute just mass matrix on each step
            Eigen::SparseMatrix<double> M;
            igl::massmatrix(V, F, igl::MASSMATRIX_TYPE_BARYCENTRIC, M);

            // Solve (M-delta*L) U = M*U
            const auto &S = (M - smoothing * L);
            Eigen::SimplicialLLT<Eigen::SparseMatrix<double>> solver(S);
            assert(solver.info() == Eigen::Success);
            V = solver.solve(M * V).eval();

            // Compute centroid and subtract (also important for numerics)
            Eigen::VectorXd dblA;
            igl::doublearea(V, F, dblA);
            double area = 0.5 * dblA.sum();
            Eigen::MatrixXd BC;
            igl::barycenter(V, F, BC);
            Eigen::RowVector3d centroid(0, 0, 0);
            for (int i = 0; i < BC.rows(); i++)
            {
                centroid += 0.5 * dblA(i) / area * BC.row(i);
            }
            V.rowwise() -= centroid;
            // Normalize to unit surface area (important for numerics)
            V.array() /= sqrt(area);
            V.rowwise() += centroid;
        }

        // Parse data
        cgeomParseMatrixXd(V, outCoords, outCoordsCount);

        // Release Eigen Matrix Memory
        F.setZero();
        V.setZero();
    }
}
