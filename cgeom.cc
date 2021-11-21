#include <iostream>
#include <igl/principal_curvature.h>


extern "C"
{
#include "cgeom.h"
}

namespace CGeom
{
    CGEOM_API void iglPrincipalCurvatures(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outDir1, double **outDir2, double **outVal1, double **outVal2)
    {
        Eigen::MatrixXd V;
        Eigen::MatrixXi F;
        V = Eigen::Map<Eigen::MatrixXd>(inCoords,numVertices, 3);
        F = Eigen::Map<Eigen::MatrixXi>(inFaces, numFaces, 3);

        // Compute curvature directions via quadric fitting
        Eigen::MatrixXd PD1,PD2;
        Eigen::VectorXd PV1,PV2;
        igl::principal_curvature(V,F,PD1,PD2,PV1,PV2);

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
        for (int i = 0; i < numVertices; i++)
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
    }
}
