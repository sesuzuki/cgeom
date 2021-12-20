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


extern "C"
{
#include "discrete_quantities.h"
}

namespace CGeom
{
    ///////////////////////////////////////////////
    // Discrete Geometric Quantities And Operators
    ///////////////////////////////////////////////

    CGEOM_QUANT_API void iglPrincipalCurvatures(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outDir1, double **outDir2, double **outVal1, double **outVal2)
    {
        // Build mesh
        Eigen::MatrixXd V;
        Eigen::MatrixXi F;
        V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        // Compute curvature directions via quadric fitting
        Eigen::MatrixXd PD1,PD2;
        Eigen::VectorXd PV1,PV2;
        igl::principal_curvature(V,F,PD1,PD2,PV1,PV2);

        // Parse data
        auto sizeVectors = (numVertices * 3) * sizeof(double);
        auto sizeValues = (numVertices) * sizeof(double);
        *outDir1 = static_cast<double *>(malloc(sizeVectors));
        *outDir2 = static_cast<double *>(malloc(sizeVectors));
        *outVal1 = static_cast<double *>(malloc(sizeValues));
        *outVal2 = static_cast<double *>(malloc(sizeValues));

        std::vector<double> dir1;
        std::vector<double> dir2;
        std::vector<double> val1;
        std::vector<double> val2;
        for(int i = 0; i < numVertices; i++)
        {
            val1.push_back(PV1(i));
            val2.push_back(PV2(i));

            dir1.push_back(PD1(i,0));
            dir1.push_back(PD1(i,1));
            dir1.push_back(PD1(i,2));
            dir2.push_back(PD2(i,0));
            dir2.push_back(PD2(i,1));
            dir2.push_back(PD2(i,2));
        }
        std::memcpy(*outVal1, val1.data(), sizeValues);
        std::memcpy(*outVal2, val2.data(), sizeValues);
        std::memcpy(*outDir1, dir1.data(), sizeVectors);
        std::memcpy(*outDir2, dir2.data(), sizeVectors);

        // Release Eigen Matrix Memory
        V.setZero();
        F.setZero();
        PD1.setZero();
        PD2.setZero();
        PV1.setZero();
        PV2.setZero();
    }

    CGEOM_QUANT_API void iglNormalsPerVertex(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outNorm){
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
    
    CGEOM_QUANT_API void iglNormalsPerFace(const int numVertices, const int numFaces, double *inCoords, int *inFaces,  double **outNorm){
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
    
    CGEOM_QUANT_API void iglNormalsPerCorner(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double angle,  double **outNorm){
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

    CGEOM_QUANT_API void iglGaussianCurvature(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outK){
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

    CGEOM_QUANT_API void iglMeanCurvature(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outH){
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

    CGEOM_QUANT_API void iglLaplacianSmoothing(const int numVertices, const int numFaces, double *inCoords, int *inFaces, int numIterations, double **outCoords){
        // Build mesh
        Eigen::MatrixXd V,U;
        Eigen::MatrixXi F;
        V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        Eigen::SparseMatrix<double> L;
        // Compute Laplace-Beltrami operator: #V by #V
        igl::cotmatrix(V,F,L);

        // Alternative construction of same Laplacian
        // Eigen::SparseMatrix<double> G,K;
        // // Gradient/Divergence
        // igl::grad(V,F,G);
        // // Diagonal per-triangle "mass matrix"
        // Eigen::VectorXd dblA;
        // igl::doublearea(V,F,dblA);
        // // Place areas along diagonal #dim times
        // const auto & T = 1.*(dblA.replicate(3,1)*0.5).asDiagonal();
        // // Laplacian K built as discrete divergence of gradient or equivalently
        // // discrete Dirichelet energy Hessian
        // K = -G.transpose() * T * G;

        U = V ;
        for(int i=0; i<numIterations; i++){
            // Recompute just mass matrix on each step
            Eigen::SparseMatrix<double> M;
            igl::massmatrix(U,F,igl::MASSMATRIX_TYPE_BARYCENTRIC,M);
            // Solve (M-delta*L) U = M*U
            const auto & S = (M - 0.001*L);
            Eigen::SimplicialLLT<Eigen::SparseMatrix<double > > solver(S);
            assert(solver.info() == Eigen::Success);
            U = solver.solve(M*U).eval();
            // Compute centroid and subtract (also important for numerics)
            Eigen::VectorXd dblA;
            igl::doublearea(U,F,dblA);
            double area = 0.5*dblA.sum();
            Eigen::MatrixXd BC;
            igl::barycenter(U,F,BC);
            Eigen::RowVector3d centroid(0,0,0);
            for(int i = 0;i<BC.rows();i++)
            {
                centroid += 0.5*dblA(i)/area*BC.row(i);
            }
            U.rowwise() -= centroid;
            // Normalize to unit surface area (important for numerics)
            U.array() /= sqrt(area);
        }

        // Parse data
        auto sizeVectors = (numVertices * 3) * sizeof(double);
        *outCoords = static_cast<double *>(malloc(sizeVectors));

        std::vector<double> coord;
        for(int i = 0; i < numVertices; i++)
        {
            coord.push_back(U(i,0));
            coord.push_back(U(i,1));
            coord.push_back(U(i,2));
        }
        std::memcpy(*outCoords, coord.data(), sizeVectors);

        // Release Eigen Matrix Memory
        V.setZero();
        F.setZero();
        U.setZero();
    }
}
