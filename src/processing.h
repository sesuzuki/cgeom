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
    CGEOM_PARAM_API void cgeomParseMatrixXd(const Eigen::MatrixXd m, double **outData, size_t *outCount);

    CGEOM_PARAM_API void cgeomParseMatrixXi(const Eigen::MatrixXd m, int **outData, size_t *outCount);

    CGEOM_PARAM_API void cgeomParseStdVectorInt(const std::vector<int> m, int **outData, size_t *outCount);

    CGEOM_PARAM_API void cgeomNRosy(const int numVertices, const int numFaces, const int numConstraints, double *inCoords, int *inFaces, 
                                    int *inConstrainedFaces, double *inConstrainedVectorFaces, int degree,
                                    size_t *outX1CoordsCount, size_t *outX2CoordsCount, size_t *outSingularitiesCount, 
                                    double **outX1Coords, double **outX2Coords, double **outSingularities);

    CGEOM_PARAM_API void cgeomNRosyFromFrameField(const int numVertices, const int numFaces, const int numConstraints, double *inCoords, int *inFaces, int *inConstrainedFaces, double *inX1, double *inX2, int degree,
                                    size_t *outX1CoordsCount, size_t *outX2CoordsCount, size_t *outSingularitiesCount, 
                                    double **outX1Coords, double **outX2Coords, double **outSingularities);

    CGEOM_PARAM_API void cgeomSeamlessIntegerGridParameterization(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double *inCoordsX1, double *inCoordsX2,
                                                                  double gradient_size, double stiffness, bool direct_round, size_t numIterations, size_t *outUVCount, size_t *outFUVCount, double **outUV, int **outFUV);

    CGEOM_PARAM_API void cgeomQuadMeshExtraction(const int inVertexCount, const int inTriasCount, const int inUVCount, const int inFUVCount, double *inCoords, int *inTrias, double *inUV, int *inFUV, 
                                              size_t *outVertexCount, size_t *outQuadsCount, double **outCoords, int **outQuads);  

    CGEOM_PARAM_API void cgeomPlanarization(const int inVertexCount, const int inQuadsCount, double *inCoords, int *inQuads, const int iterations, const double threshold, size_t *outVertexCount, size_t *outPlanarityCount, double **outCoords, double **outPlanarity);

    CGEOM_PARAM_API void cgeomRotateVectors(const int numVectors, double *inX1Coords, double *inB1Coords, double *inB2Coords, double *inAngle, size_t *outCount, double **outX1Coords);

    CGEOM_PARAM_API int cgeomParallelTransport(const int numVertices, const int numFaces, double *inCoords, int *inFaces, const int inSourceVertex, const double initialPara, const double initialPerp, size_t *outCount, double **outCoords, double **outVecCoords, const char **errorMessage);

    CGEOM_PARAM_API int cgeomEdgeVectors(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t *outCount, double **outEdgeMidCoords, double **outParCoords, double **outPerpCoords, const char **errorMessage);

    CGEOM_PARAM_API int cgeomRemeshAlongIsoline(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double *inScalarField, double inIsoValue, size_t *outVertexCount, size_t *outFaceCount, double **outCoords, int **outFaces, const char **errorMessage);
}