#define CGEOM_PARAM_API_VERSION "cgeom"

#if defined(CGEOM_PARAM_DLL)
#if defined(CGEOM_PARAM_DLL_EXPORT)
#define CGEOM_PARAM_API __declspec(dllexport)
#else
#define CGEOM_PARAM_API __declspec(dllimport)
#endif
#else
#define CGEOM_PARAM_API __attribute__((visibility("default")))
#endif

namespace CGeom
{
    CGEOM_PARAM_API void cgeomParseMatrixXd(const Eigen::MatrixXd m, double **outData);

    CGEOM_PARAM_API void cgeomParseMatrixXi(const Eigen::MatrixXd m, int **outData);

    CGEOM_PARAM_API void cgeomNRosy(const int numVertices, const int numFaces, const int numConstraints, double *inCoords, int *inFaces, int *inConstrainedFaces, double *inConstrainedVectorFaces, 
                                    int degree, double **outX1Coords, double **outX2Coords, double **outBarycentersCoords, double **outSingularities);

    CGEOM_PARAM_API void cgeomSeamlessIntegerGridParameterization(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double *inCoordsX1, double *inCoordsX2,
                                                                  double gradient_size, double stiffness, bool direct_round, size_t numIterations, size_t *outNumUV, size_t *outNumFUV, double **outUV, int **outFUV);

    CGEOM_PARAM_API void cgeomQuadMeshExtraction(const int inVertexCount, const int inTriasCount, double *inCoords, int *inTrias, double *inUV, int *inFUV, 
                                              size_t *outVertexCount, size_t *outQuadsCount, double **outCoords, int **outQuads);  

}