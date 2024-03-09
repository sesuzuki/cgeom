#include "bff/mesh/MeshIO.h"
#include "bff/mesh/PolygonSoup.h"
#include "bff/mesh/Mesh.h"
#include "bff/project/HoleFiller.h"
#include "bff/project/Generators.h"
#include "bff/project/Bff.h"

extern "C"
{
#include "bff.h"
}

namespace CGeom
{
    // Current implementation only works for open surfaces with one boundary
    CGEOM_BFF_API void cgeomOpenSurfaceWithoutHolesBFF(const int numVertices, const int numFaces, const double *inCoords, const int *inFaces, int *outNumUV, double **outUV, int *outNumFaces, int **outFaces, const char **errorMessage)    {
        try
        {
            bff::PolygonSoup soup;
            std::vector<std::pair<int, int>> uncuttableEdges;

            // Build soup
            for (int i = 0; i < numVertices; i++) soup.positions.emplace_back(bff::Vector(inCoords[i * 3], inCoords[i * 3 + 1], inCoords[i * 3 + 2]));
            for (int i = 0; i < numFaces; i++) {
                soup.indices.emplace_back(inFaces[i*3]);
                soup.indices.emplace_back(inFaces[i*3+1]);
                soup.indices.emplace_back(inFaces[i*3+2]);
            }

            bff::Model model;
            bff::MeshIO::buildModel(uncuttableEdges, soup, model, error);

            // Initialize BFF
            std::vector<uint8_t> isSurfaceMappedToSphere;
            bff::Mesh* mesh = NULL;
		    for (int i = model.size() - 1; i >= 0; i--) 
            {
                mesh = &model[i];

                // flatten with isometric lengths
                auto bff = std::shared_ptr<bff::BFF>(new bff::BFF(*mesh));

                // Flatten to disk
                bff::DenseMatrix u(bff->data->bN);
                bff->flattenToDisk();
                mesh->projectUvsToPcaAxis();
                isSurfaceMappedToSphere.emplace_back(0);
			}

            // Parse disk mesh
            std::vector<bff::Vector> originalUvIslandCenters, newUvIslandCenters;
            std::vector<uint8_t> isUvIslandFlipped;
            bff::Vector modelMinBounds, modelMaxBounds;
            bff::MeshIO::packUvs(model, 1.0, isSurfaceMappedToSphere, originalUvIslandCenters,
                    newUvIslandCenters, isUvIslandFlipped, modelMinBounds, modelMaxBounds);

            // collect model UVs
            std::vector<bff::Vector> positions, uvs;
            std::vector<int> vIndices, uvIndices, indicesOffset;
            bff::MeshIO::collectModelUvs(model, true, isSurfaceMappedToSphere,
                            originalUvIslandCenters, newUvIslandCenters,
                            isUvIslandFlipped, modelMinBounds, modelMaxBounds,
                            positions, uvs, vIndices, uvIndices, indicesOffset);

            // Parse Positions
            std::vector<double> outCoords; 
            for (int i = 0; i < (int)uvs.size(); i++) 
            {
                const bff::Vector& uv = uvs[i];
                outCoords.emplace_back(uv.x);
                outCoords.emplace_back(uv.y);
                outCoords.emplace_back(0.0);
            }

            *outNumUV = outCoords.size();
            auto sF = *outNumUV * sizeof(double);
            *outUV = static_cast<double *>(malloc(sF));
            std::memcpy(*outUV, outCoords.data(), sF);

            // Parse faces
            std::vector<int> outFaceIndices;
            for (int i = 0; i < (int)indicesOffset.size() - 1; i++) 
            {
                for (int j = indicesOffset[i]; j < indicesOffset[i + 1]; j++) outFaceIndices.emplace_back(uvIndices[j]);
            }

            *outNumFaces = outFaceIndices.size();
            auto cF = *outNumFaces * sizeof(int);
            *outFaces = static_cast<int *>(malloc(cF));
            std::memcpy(*outFaces, outFaceIndices.data(), cF);

            *errorMessage  = "Success";

        } catch(...) {
            *errorMessage  = "Error in bff";
        }
    }
}