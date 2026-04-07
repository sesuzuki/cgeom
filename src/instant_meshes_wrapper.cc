// This translation unit includes Instant Meshes headers, which inject
// VectorXi, MatrixXf etc. into the global namespace.  Keeping them in a
// separate .cc file prevents collisions with Eigen in processing.cc.

// Instant Meshes defines nprocs in main.cpp (excluded from our build).
// Provide the definition here so field.cpp's extern reference resolves.
int nprocs = -1;  // -1 = TBB auto (all available cores)

#include "instant_meshes_wrapper.h"

// Instant Meshes core (no GUI headers included)
#include "common.h"
#include "dedge.h"
#include "subdivide.h"
#include "meshstats.h"
#include "hierarchy.h"
#include "field.h"
#include "normal.h"
#include "adjacency.h"
#include "extract.h"
#include "bvh.h"

#include <stdexcept>
#include <set>

std::string instantMeshesProcess(
    const double* inCoords, int numVertices,
    const int*    inFaces,  int numFaces,
    int rosy, int posy, float scale, int targetVertexCount,
    std::vector<float>& outVertices,
    std::vector<int>&   outFaceIndices,
    std::vector<int>&   outFaceDegrees,
    float& outActualScale)
{
    try {
            
        // ---- Build Instant Meshes MatrixXf V (3 x nV) and MatrixXu F (3 x nF) ----
        MatrixXf V(3, numVertices);
        for (int i = 0; i < numVertices; ++i) {
            V(0, i) = (float)inCoords[3*i + 0];
            V(1, i) = (float)inCoords[3*i + 1];
            V(2, i) = (float)inCoords[3*i + 2];
        }

        MatrixXu F(3, numFaces);
        for (int i = 0; i < numFaces; ++i) {
            F(0, i) = (uint32_t)inFaces[3*i + 0];
            F(1, i) = (uint32_t)inFaces[3*i + 1];
            F(2, i) = (uint32_t)inFaces[3*i + 2];
        }

        // ---- Mesh stats ----
        bool deterministic = false;
        MeshStats stats = compute_mesh_stats(F, V, deterministic);
        int vertex_count = targetVertexCount, face_count = -1;
        scale = -1;
        if (scale < 0 && vertex_count < 0 && face_count < 0) {
            cout << "No target vertex count/face count/scale argument provided. "
                    "Setting to the default of 1/16 * input vertex count." << endl;
            vertex_count = V.cols() / 16;
        }

        if (scale > 0) {
            Float face_area = posy == 4 ? (scale*scale) : (std::sqrt(3.f)/4.f*scale*scale);
            face_count = stats.mSurfaceArea / face_area;
            vertex_count = posy == 4 ? face_count : (face_count / 2);
        } else if (face_count > 0) {
            Float face_area = stats.mSurfaceArea / face_count;
            vertex_count = posy == 4 ? face_count : (face_count / 2);
            scale = posy == 4 ? std::sqrt(face_area) : (2*std::sqrt(face_area * std::sqrt(1.f/3.f)));
        } else if (vertex_count > 0) {
            face_count = posy == 4 ? vertex_count : (vertex_count * 2);
            Float face_area = stats.mSurfaceArea / face_count;
            scale = posy == 4 ? std::sqrt(face_area) : (2*std::sqrt(face_area * std::sqrt(1.f/3.f)));
        }

        MultiResolutionHierarchy mRes;
        MatrixXf N;
        VectorXf A;
        std::set<uint32_t> crease_in, crease_out;
        AdjacencyMatrix adj = nullptr;
        Float creaseAngle = -1.f;  // disabled

        ///////////////////////////////////////////////////////////////////////
         /* Subdivide the mesh if necessary */
        VectorXu V2E, E2E;
        VectorXb boundary, nonManifold;
        if (stats.mMaximumEdgeLength*2 > scale || stats.mMaximumEdgeLength > stats.mAverageEdgeLength * 2) {
            cout << "Input mesh is too coarse for the desired output edge length "
                    "(max input mesh edge length=" << stats.mMaximumEdgeLength
                 << "), subdividing .." << endl;
            build_dedge(F, V, V2E, E2E, boundary, nonManifold);
            subdivide(F, V, V2E, E2E, boundary, nonManifold, std::min(scale/2, (Float) stats.mAverageEdgeLength*2), deterministic);
        }

        /* Compute a directed edge data structure */
        build_dedge(F, V, V2E, E2E, boundary, nonManifold);

        /* Compute adjacency matrix */
        adj = generate_adjacency_matrix_uniform(F, V2E, E2E, nonManifold);

        // /* Compute vertex/crease normals */
        if (creaseAngle >= 0)
            generate_crease_normals(F, V, V2E, E2E, boundary, nonManifold, creaseAngle, N, crease_in);
        else
            generate_smooth_normals(F, V, V2E, E2E, nonManifold, N);

        /* Compute dual vertex areas */
        compute_dual_vertex_areas(F, V, V2E, E2E, nonManifold, A);

        mRes.setE2E(std::move(E2E));
        ///////////////////////////////////////////////////////////////////////

        mRes.setAdj(std::move(adj));
        mRes.setF(std::move(F));
        mRes.setV(std::move(V));
        mRes.setA(std::move(A));
        mRes.setN(std::move(N));
        mRes.setScale(scale);
        mRes.build(deterministic);
        mRes.resetSolution();

        mRes.clearConstraints();
        for (uint32_t i=0; i<3*mRes.F().cols(); ++i) 
        {
            if (mRes.E2E()[i] == INVALID) {
                uint32_t i0 = mRes.F()(i%3, i/3);
                uint32_t i1 = mRes.F()((i+1)%3, i/3);
                Vector3f p0 = mRes.V().col(i0), p1 = mRes.V().col(i1);
                Vector3f edge = p1-p0;
                if (edge.squaredNorm() > 0) {
                    edge.normalize();
                    mRes.CO().col(i0) = p0;
                    mRes.CO().col(i1) = p1;
                    mRes.CQ().col(i0) = mRes.CQ().col(i1) = edge;
                    mRes.CQw()[i0] = mRes.CQw()[i1] = mRes.COw()[i0] =
                        mRes.COw()[i1] = 1.0f;
                }
            }
        }
        mRes.propagateConstraints(rosy, posy);

        // ---- BVH for snapping (improves output quality) ----
        BVH* bvh = new BVH(&mRes.F(), &mRes.V(), &mRes.N(), stats.mAABB);
        bvh->build();

        // ---- Orientation and position field optimisation ----
        Optimizer optimizer(mRes, false);
        optimizer.setRoSy(rosy);
        optimizer.setPoSy(posy);
        optimizer.setExtrinsic(false);

        optimizer.optimizeOrientations(-1);
        optimizer.notify();
        optimizer.wait();

        optimizer.optimizePositions(-1);
        optimizer.notify();
        optimizer.wait();

        optimizer.shutdown();

        // ---- Extract output polygonal mesh ----
        MatrixXf O_extr, N_extr, Nf_extr;
        std::vector<std::vector<TaggedLink>> adj_extr;
        extract_graph(mRes, false, rosy, posy, adj_extr, O_extr, N_extr,
                      crease_in, crease_out, deterministic);

        MatrixXu F_extr;
        bool pure = (posy == 4);  // pure quads for posy=4; mixed for posy=3
        extract_faces(adj_extr, O_extr, N_extr, Nf_extr, F_extr, posy,
                      mRes.scale(), crease_out, true, pure);//, bvh, 2);

        // ---- Pack output ----
        // O_extr is 3 x nV (column-major)
        //MatrixXf O_extr = V;  // placeholder until extraction is implemented
        int nVout = O_extr.cols();
        outVertices.resize(3 * nVout);
        for (int i = 0; i < nVout; ++i) {
            outVertices[3*i + 0] = O_extr(0, i);
            outVertices[3*i + 1] = O_extr(1, i);
            outVertices[3*i + 2] = O_extr(2, i);
        }

        // F_extr is rows x nF (column-major); rows = degree (3 or 4)
        // Instant Meshes marks degenerate quads (triangles stored as quads)
        // by repeating the last vertex: F(2,f)==F(3,f).
        //MatrixXu F_extr = F;  // placeholder until extraction is implemented
        int degree = 3;//F_extr.rows();
        int nFout  = F_extr.cols();
        for (int f = 0; f < nFout; ++f) {
            if (degree == 4 && F_extr(2, f) == F_extr(3, f)) {
                // degenerate quad -> triangle
                outFaceDegrees.push_back(3);
                outFaceIndices.push_back((int)F_extr(0, f));
                outFaceIndices.push_back((int)F_extr(1, f));
                outFaceIndices.push_back((int)F_extr(2, f));
            } else {
                outFaceDegrees.push_back(degree);
                for (int j = 0; j < degree; ++j)
                    outFaceIndices.push_back((int)F_extr(j, f));
            }
        }

        if (bvh) delete bvh;

        return "";  // success
    }
    catch (const std::exception& e) {
        return std::string(e.what());
    }
    catch (...) {
        return "instantMeshesProcess: unknown exception";
    }
}
