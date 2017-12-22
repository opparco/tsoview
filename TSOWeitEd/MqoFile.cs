using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D = Microsoft.DirectX.Direct3D;

namespace TDCG
{
    using NavGraph = SparseGraph<MqoVert, MqoEdge>;

    /// <summary>
    /// 統合メッシュ
    /// </summary>
    public class MqoMesh
    {
        /// <summary>
        /// 頂点配列
        /// </summary>
        public MqoVert[] vertices;

        TSOMesh mesh = null;

        static Dictionary<TSOMesh, MqoMesh> meshmap = new Dictionary<TSOMesh, MqoMesh>();

        public NavGraph graph = new NavGraph(false);//not a digraph

        public Graph_SearchDijkstra djk = null;

        internal void CreatePathDijkstra(int sourceCell, int targetCell)
        {
            djk = new Graph_SearchDijkstra(graph, sourceCell, targetCell);
        }

        internal static MqoMesh FromTSOMesh(TSOMesh mesh)
        {
            MqoMesh mqo_mesh;
            if (meshmap.TryGetValue(mesh, out mqo_mesh))
                return mqo_mesh;

            mqo_mesh = new MqoMesh(mesh);
            mqo_mesh.CreateVertices();
            meshmap[mesh] = mqo_mesh;
            
            return mqo_mesh;
        }

        public MqoMesh(TSOMesh mesh)
        {
            this.mesh = mesh;
        }

        internal void CreateVertices()
        {
            Heap<MqoVert> mh = new Heap<MqoVert>();

            foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
            {
                int cnt = 0;
                ushort a, b = 0, c = 0;
                MqoVert ma, mb = new MqoVert(), mc = new MqoVert();

                foreach (Vertex v in sub_mesh.vertices)
                {
                    MqoVert m = new MqoVert(v.position, v.normal, null);
                    ushort i;
                    if (mh.map.TryGetValue(m, out i))
                    {
                        //集約先はidx=i
                        m = mh.ary[i];
                    }
                    else
                    {
                        m.skin_weights = new MqoSkinWeight[4];
                        for (int w = 0; w < 4; w++)
                        {
                            m.skin_weights[w] = new MqoSkinWeight(sub_mesh.GetBone(v.skin_weights[w].bone_index), v.skin_weights[w].weight);
                        }
                        //mはidx=iになる
                        i = (ushort)mh.Count;
                        mh.Add(m);
                    }
                    //集約する
                    m.rel.Add(new TSOPair(v, sub_mesh));

                    cnt++;
                    a = b;
                    b = c;
                    c = i;
                    ma = mb;
                    mb = mc;
                    mc = m;

                    m.Index = i;
                    graph.AddNode(m);

                    if (cnt < 3)
                        continue;

                    if (a != b && b != c && c != a)
                    {
                        graph.AddEdge(new MqoEdge(a, b, GetCost(ma, mb)));
                        graph.AddEdge(new MqoEdge(b, c, GetCost(mb, mc)));
                        graph.AddEdge(new MqoEdge(c, a, GetCost(mc, ma)));
                    }
                }
            }
            vertices = mh.ary.ToArray();
        }

        internal static float GetCost(MqoVert ma, MqoVert mb)
        {
            float dx = mb.position.X - ma.position.X;
            float dy = mb.position.Y - ma.position.Y;
            float dz = mb.position.Z - ma.position.Z;
            return dx*dx + dy*dy + dz*dz;
        }

        internal void Update()
        {
            foreach (MqoVert v in vertices)
            {
                v.Update();
            }
        }

        internal void CalcSkindeform(Matrix [] clipped_boneMatrices)
        {
            foreach (MqoVert v in vertices)
            {
                v.deformed_position = v.CalcSkindeformPosition(clipped_boneMatrices);
                v.deformed_normal = v.CalcSkindeformNormal(clipped_boneMatrices);
            }
        }

        internal void WriteBuffer()
        {
            foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
                sub_mesh.WriteBuffer();
        }
    }

    public class TSOPair
    {
        public Vertex a;
        public TSOSubMesh sub_mesh;

        public TSOPair(Vertex a, TSOSubMesh sub_mesh)
        {
            this.a = a;
            this.sub_mesh = sub_mesh;
        }
    }

    /// <summary>
    /// 統合頂点
    /// </summary>
    public class MqoVert : GraphNode
    {
        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// 法線
        /// </summary>
        public Vector3 normal;
        /// <summary>
        /// スキンウェイト配列
        /// </summary>
        public MqoSkinWeight[] skin_weights;

        /// 選択中であるか
        public bool selected = false;

        /// ウェイト乗数
        public float factor;

        public List<TSOPair> rel = new List<TSOPair>();

        public Vector3 deformed_position;
        public Vector3 deformed_normal;

        public MqoVert()
        {
        }

        public MqoVert(Vector3 position, Vector3 normal, MqoSkinWeight[] skin_weights)
        {
            this.position = position;
            this.normal = normal;
            this.skin_weights = skin_weights;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode() ^ normal.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is MqoVert)
            {
                MqoVert o = (MqoVert)obj;
                return position.Equals(o.position) && normal.Equals(o.normal);
            }
            return false;
        }

        public bool Equals(MqoVert o)
        {
            if ((object)o == null)
            {
                return false;
            }

            return position.Equals(o.position) && normal.Equals(o.normal);
        }

        internal Vector3 CalcSkindeformPosition(Matrix[] bone_matrices)
        {
            Vector3 pos = Vector3.Empty;
            for (int i = 0; i < 4; i++)
            {
                Matrix m = bone_matrices[skin_weights[i].bone.Id];
                float w = skin_weights[i].weight;
                pos += Vector3.TransformCoordinate(position, m) * w;
            }
            return pos;
        }

        internal Vector3 CalcSkindeformNormal(Matrix[] bone_matrices)
        {
            Vector3 nor = Vector3.Empty;
            for (int i = 0; i < 4; i++)
            {
                Matrix m = bone_matrices[skin_weights[i].bone.Id];
                m.M41 = 0;
                m.M42 = 0;
                m.M43 = 0;
                float w = skin_weights[i].weight;
                nor += Vector3.TransformCoordinate(normal, m) * w;
            }
            return nor;
        }

        internal static float WeightEpsilon = float.Epsilon;

        internal void Update()
        {
            foreach (TSOPair p in rel)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (skin_weights[i].weight < WeightEpsilon)
                    {
                        p.a.skin_weights[i] = new SkinWeight(0, 0.0f);
                    }
                    else
                    {
                        TSONode bone = skin_weights[i].bone;
                        int bone_idx = p.sub_mesh.bones.IndexOf(bone);
                        if (bone_idx == -1)
                            bone_idx = p.sub_mesh.AddBone(bone);
                        if (bone_idx != -1)
                            p.a.skin_weights[i] = new SkinWeight(bone_idx, skin_weights[i].weight);
                    }
                }
                p.a.FillSkinWeights();
                p.a.GenerateBoneIndices();
            }
        }
    }

    /// <summary>
    /// 統合スキンウェイト
    /// </summary>
    public class MqoSkinWeight : IComparable
    {
        /// <summary>
        /// node (bone)
        /// </summary>
        public TSONode bone;

        /// <summary>
        /// ウェイト
        /// </summary>
        public float weight;

        public MqoSkinWeight(TSONode bone, float weight)
        {
            this.bone = bone;
            this.weight = weight;
        }

        /// <summary>
        /// 比較関数
        /// </summary>
        /// <param name="obj">比較対象スキンウェイト</param>
        /// <returns>比較結果</returns>
        public int CompareTo(object obj)
        {
            return -weight.CompareTo(((MqoSkinWeight)obj).weight);
        }
    }

    public class MqoEdge : GraphEdge
    {
        public MqoEdge()
        {
        }

        public MqoEdge(int from, int to, float cost)
        {
            this.From = from;
            this.To = to;
            this.Cost = cost;
        }
    }
}
