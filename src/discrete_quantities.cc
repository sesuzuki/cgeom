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
#include <igl/local_basis.h>

extern "C"
{
#include "discrete_quantities.h"
#include "processing.h"
}

namespace CGeom
{
    ///////////////////////////////////////////////
    // Discrete Geometric Quantities And Operators
    ///////////////////////////////////////////////

    CGEOM_QUANT_API void cgeomLocalBasis(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t *outCount, double **outX1Coords, double **outX2Coords, double **outX3Coords){
        // Build mesh
        Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        Eigen::MatrixXd B1, B2, B3;
        igl::local_basis(V, F, B1, B2, B3);

        cgeomParseMatrixXd(B1, outX1Coords, outCount);
        cgeomParseMatrixXd(B2, outX2Coords, outCount);
        cgeomParseMatrixXd(B3, outX3Coords, outCount);

        B1.setZero();
        B2.setZero();
        B3.setZero();
    }

    CGEOM_QUANT_API int cgeomPerVertexAsymptoticDirections(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t *outX1Count, size_t *outX2Count, size_t *outVanishingCount, double **outX1, double **outX2, int **outVanishing, const char **errorMessage){ 
        try{
            // Build mesh
            Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
            Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

            // Compute curvature directions via quadric fitting
            Eigen::MatrixXd x1,x2;
            Eigen::VectorXd k1,k2;
            igl::principal_curvature(V,F,x1,x2,k1,k2);

            // Compute asymptotic directions
            Eigen::MatrixXd aX1(numVertices,3), aX2(numVertices,3);
            std::vector<int> aVan;
            for(int i=0; i<numVertices; i++){
                const auto vk1 = k1(i);
                const auto vk2 = k2(i);

                if(vk1 * vk2 > 0 || (vk1 * vk2 == 0 && vk1==vk2)) {
                    aX1.row(i) = Eigen::Vector3d(0,0,0);
                    aVan.push_back(i); 
                }
                else
                {
                    const double cos = sqrt(1 - (vk1 / (vk1 - vk2)));
                    const double sin = sqrt((vk1/ (vk1 - vk2)));
                    aX1.row(i) = (cos * x1.row(i) + sin * x2.row(i)).normalized();
                    aX2.row(i) = (cos * x1.row(i) - sin * x2.row(i)).normalized();
                }
            }

            // Parse data
            cgeomParseMatrixXd(aX1, outX1, outX1Count);
            cgeomParseMatrixXd(aX2, outX2, outX2Count);
            cgeomParseStdVectorInt(aVan, outVanishing, outVanishingCount);

            // Release Eigen Matrix Memory
            F.setZero();
            V.setZero();
            x1.setZero();
            x2.setZero();
            aX1.setZero();
            aX2.setZero();
            k1.setZero();
            k2.setZero();

            *errorMessage = "Success";
            return 0;
        }
        catch (const std::runtime_error &error)
        {
            *errorMessage = error.what();
            return 1;
        }
    }

    CGEOM_QUANT_API void cgeomBarycenters(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t *outCoordsCount, double **outCoords)
    {
        // Build mesh
        Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords, numVertices, 3);
        Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        Eigen::MatrixXd B;
        igl::barycenter(V, F, B);

        cgeomParseMatrixXd(B, outCoords, outCoordsCount);
    }

    CGEOM_QUANT_API int cgeomPerFaceAsymptoticDirections(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t *outX1Count, size_t *outX2Count, size_t *outVanishingCount, double **outX1Coords, double **outX2Coords, int **outVanishing, const char **errorMessage){ 
        try{
            // Build mesh
            Eigen::MatrixXd V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
            Eigen::MatrixXi F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

            // Compute per vertex curvature directions via quadric fitting
            Eigen::MatrixXd vX1,vX2;
            Eigen::VectorXd vK1,vK2;
            igl::principal_curvature(V,F,vX1,vX2,vK1,vK2);

            // Compute per face curvature directions as the average of vertex values
            Eigen::MatrixXd fX1(numFaces, 3);
            Eigen::VectorXd thetas(numFaces);
            std::vector<int> aVan;
            for(int i=0; i<numFaces; i++)
            {
                Eigen::Vector3d x1 = vX1.row(F.coeff(i,0));
                double k1 = 0, k2 = 0;

                for(int j=1; j<3; j++)
                {
                    const auto idx = F.coeff(i,j);
                    double dot = x1.dot(vX1.row(idx));
                    if(dot>0) dot = 1;
                    else if(dot<0) dot = -1;
                    x1 += dot * vX1.row(idx);
                    k1 += vK1(idx);
                    k2 += vK2(idx);
                }

                x1.normalize();
                k1 /= 3;
                k2 /= 3;

                // Compute assymptotic direction
                if(k1 * k2 > 0 || (k1 * k2 == 0 && k1==k2)) 
                {
                    fX1.row(i) = Eigen::Vector3d(0,0,0);
                    aVan.push_back(i); 
                }
                else
                {
                    thetas(i) = 2 * atan(sqrt((2 * sqrt(k2 * (k2 - k1)) + k1 - 2 * k2) / k1));
                    fX1.row(i) = x1;
                }
            }

            Eigen::MatrixXd B1, B2, B3;
            igl::local_basis(V, F, B1, B2, B3);
            Eigen::MatrixXd fA1 = igl::rotate_vectors(fX1, thetas, B1, B2);
            Eigen::MatrixXd fA2 = igl::rotate_vectors(fA1, Eigen::VectorXd::Constant(1, igl::PI / 2), B1, B2);

            // Parse data
            cgeomParseMatrixXd(fA1, outX1Coords, outX1Count);
            cgeomParseMatrixXd(fA2, outX2Coords, outX2Count);
            cgeomParseStdVectorInt(aVan, outVanishing, outVanishingCount);

            // Release Eigen Matrix Memory
            F.setZero();
            V.setZero();
            vX1.setZero();
            vX2.setZero();
            fX1.setZero();
            fA1.setZero();
            fA1.setZero();
            vK1.setZero();
            vK2.setZero();
            B1.setZero();
            B2.setZero();
            B3.setZero();

            *errorMessage = "Success";
            return 0;
        }
        catch (const std::runtime_error &error)
        {
            *errorMessage = error.what();
            return 1;
        }
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
            Eigen::Vector3d x1(0,0,0), x2(0,0,0);
            double k1 = 0, k2 = 0;
            for(int j=0; j<3; j++){
                x1 += vX1.row(F.coeff(i,j));
                x2 += vX2.row(F.coeff(i,j));
                k1 += vK1(F.coeff(i,j));
                k2 += vK2(F.coeff(i,j));
            }

            fX1.row(i) = x1.normalized();
            fX2.row(i) = x2.normalized();
            fK1(i) = k1 / 3;
            fK2(i) = k2 / 3;
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
