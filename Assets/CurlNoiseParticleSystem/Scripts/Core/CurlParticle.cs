using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using CurlNoiseParticleSystem.Utility;

namespace CurlNoiseParticleSystem
{
    public enum CurlNoiseType
    {
        Normal = 0,
        Fake,
    }

    /// <summary>
    /// This struct is used as same shader and compute shader struct.
    /// </summary>
    public struct Particle
    {
        public int id;
        public int active;
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 color;
        public float scale;
        public float baseScale;
        public float time;
        public float liefTime;
        public float delay;
    }

    /// <summary>
    /// To use particle parameter for emitting.
    /// </summary>
    public struct ParticleParam
    {
        public Vector3 Position;
        public Vector3 Color;
        public float Delay;
    }

    /// <summary>
    /// Curl noise based particle.
    /// </summary>
    public class CurlParticle : CacheBehaviour
    {
        private readonly Quaternion ROT_XZ_PLANE = Quaternion.Euler(90f, 0, 0);

        public System.Action<CurlParticle> OnStop { get; set; }

        #region ### パーティクル設定 ###
        [Header("==== パーティクル設定 ====")]
        [SerializeField]
        [Tooltip("パーティクルの生成上限")]
        private int _particleNumLimit = 10000;

        [SerializeField]
        [Tooltip("ランダムの最小ライフタイム")]
        private float _minLifeTime = 1f;

        [SerializeField]
        [Tooltip("ランダムの最大ライフタイム")]
        private float _maxLifeTime = 5f;

        [SerializeField]
        [Tooltip("ベースのスケール")]
        private float _baseScale = 0.001f;

        [SerializeField]
        private float _randomRange = 0.01f;

        [SerializeField]
        [Tooltip("パーティクルに利用するメッシュ")]
        private Mesh _mesh;

        [SerializeField]
        private Shader _shader;
        #endregion ### パーティクル設定 ###

        [SerializeField]
        ComputeShader _computeShader;

        [SerializeField]
        private CurlParticleProfile _curlParticleProfile;

        [Header("==== デバッグツール ====")]
        [SerializeField]
        private bool _showGizmos = true;

        #region ### Private fields ###
        private int[] _p;
        private ComputeBuffer _gridData;
        private Mesh _combinedMesh;
        private Material _material;
        private List<MaterialPropertyBlock> _propertyBlocks = new List<MaterialPropertyBlock>();

        private Particle[] _particles;
        private ComputeBuffer _particlesBuffer;

        private int[] _particleArgs;
        private ComputeBuffer _particleArgsBuffer;
        private ComputeBuffer _particlePoolBuffer;

        private int _initKernel;
        private int _stopKernel;
        private int _emitKernel;
        private int _curlnoiseKernel;

        private int _particleNumPerMesh;
        private int _meshNum;

        private bool _isActive = false;
        private bool _isInitialized = false;

        private bool _autoRelease = true;
        public bool AutoRelease
        {
            get { return _autoRelease; }
            set { _autoRelease = value; }
        }

        private float _lifeTime = 0;

        private readonly Vector3 VECTOR3_ZERO = Vector3.zero;

        private Vector3 ColorVec
        {
            get
            {
                return new Vector3(_curlParticleProfile.ParticleColor.r, _curlParticleProfile.ParticleColor.g, _curlParticleProfile.ParticleColor.b);
            }
        }

        private ParticleParam DefaultParam
        {
            get
            {
                return new ParticleParam
                {
                    Color = ColorVec,
                    Position = VECTOR3_ZERO,
                    Delay = 0,
                };
            }
        }

        #region ### ShaderNameID ###
        private int _useFakeId;
        private int _octavesId;
        private int _frequencyId;
        private int _pId;
        private int _noiseScalesId;
        private int _noiseGainId;
        private int _risingForceId;
        private int _timeId;
        private int _plumeBaseId;
        private int _plumeHeightId;
        private int _plumeCeilingId;
        private int _ringRadiusId;
        private int _ringMagnitudeId;
        private int _ringFalloffId;
        private int _ringSpeedId;
        private int _ringPerSecondId;
        private int _curlNoiseIntencityId;
        private int _speedFactorId;
        private int _particlesId;
        private int _deadListId;
        private int _deltaTimeId;
        private int _idOffsetId;
        private int _particlePoolId;
        private int _positionId;
        private int _colorId;
        private int _minLifeTimeId;
        private int _maxLifeTimeId;
        private int _delayId;
        private int _scaleId;
        private int _baseScaleId;
        #endregion ### ShaderNameID ###
        #endregion ### Private fields ###

        #region ### MonoBehaviour ###
        private void Awake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void Update()
        {
            if (!_isActive)
            {
                return;
            }

            if (!_isInitialized)
            {
                return;
            }

            _lifeTime -= Time.deltaTime;
            if (_lifeTime <= 0)
            {
                Deactivate();
                return;
            }

            UpdatePosition(Camera.main);
#if UNITY_EDITOR
            if (UnityEditor.SceneView.lastActiveSceneView != null)
            {
                UpdatePosition(UnityEditor.SceneView.lastActiveSceneView.camera);
            }
#endif
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_showGizmos)
            {
                return;
            }

            Color[] colors = new[] { Color.blue, Color.green, Color.red, Color.cyan, Color.magenta, };

            for (int i = 0; i < _curlParticleProfile.NoiseScales.Length; i++)
            {
                Gizmos.color = colors[i % colors.Length];
                Gizmos.DrawWireCube(transform.position, Vector3.one * _curlParticleProfile.NoiseScales[i]);
            }


            // Base and height.
            {
                // Draw base
                UnityEditor.Handles.color = Gizmos.color = Color.red;

                Vector3 basePos = transform.position + Vector3.up * _curlParticleProfile.PlumeBase;
                UnityEditor.Handles.RectangleHandleCap(0, basePos, ROT_XZ_PLANE, 0.5f, EventType.Repaint);

                Gizmos.DrawWireSphere(basePos, 0.01f);

                // Draw height
                UnityEditor.Handles.color = Gizmos.color = Color.blue;

                Vector3 heightPos = basePos + Vector3.up * _curlParticleProfile.PlumeHeight;
                UnityEditor.Handles.RectangleHandleCap(0, heightPos, ROT_XZ_PLANE, 0.5f, EventType.Repaint);

                Gizmos.DrawWireSphere(heightPos, 0.01f);

                // Line
                Gizmos.color = Color.white;
                Gizmos.DrawLine(basePos, heightPos);
            }

            // Ceiling
            {
                Vector3 ceilingPos = transform.position + Vector3.up * _curlParticleProfile.PlumeCeiling;

                UnityEditor.Handles.color = Color.cyan;
                UnityEditor.Handles.CircleHandleCap(0, ceilingPos, ROT_XZ_PLANE, 0.5f, EventType.Repaint);
            }
        }
#endif
        #endregion ### MonoBehaviour ###

        /// <summary>
        /// アクティブ
        /// </summary>
        private void Activate()
        {
            _isActive = true;
        }

        /// <summary>
        /// 非アクティブ
        /// </summary>
        private void Deactivate()
        {
            _isActive = false;

            if (AutoRelease)
            {
                Release();
            }
        }

        /// <summary>
        /// Release to the pool.
        /// </summary>
        public void Release()
        {
            OnStop.SafeInvoke(this);
        }

        /// <summary>
        /// プールの残りサイズを取得する
        /// </summary>
        /// <returns></returns>
        private int GetParticlePoolSize()
        {
            _particleArgsBuffer.SetData(_particleArgs);
            ComputeBuffer.CopyCount(_particlePoolBuffer, _particleArgsBuffer, 0);
            _particleArgsBuffer.GetData(_particleArgs);

            return _particleArgs[0];
        }

        /// <summary>
        /// Stop the particle.
        /// </summary>
        public void Stop()
        {
            DispatchStop();
        }

        /// <summary>
        /// Compute Shaderを使って位置を更新する
        /// </summary>
        private void UpdatePosition(Camera camera)
        {
            float frequency = Mathf.Clamp(_curlParticleProfile.Frequency, 0.1f, 64.0f);
            int octaves = Mathf.Clamp(_curlParticleProfile.Octaves, 1, 16);

            if (_gridData == null)
            {
                _gridData = new ComputeBuffer(512, sizeof(int));
                _gridData.SetData(_p);
            }

            _computeShader.SetInt(_octavesId, octaves);
            _computeShader.SetFloat(_frequencyId, frequency);
            _computeShader.SetBuffer(_curlnoiseKernel, _pId, _gridData);

            #region ### カールノイズパラメータ ###
            _computeShader.SetFloats(_noiseScalesId, _curlParticleProfile.NoiseScales);
            _computeShader.SetFloats(_noiseGainId, _curlParticleProfile.NoiseGain);

            Vector3 rf = _curlParticleProfile.RisingForce;
            _computeShader.SetFloats(_risingForceId, new[] { rf.x, rf.y, rf.z });

            _computeShader.SetBool(_useFakeId, _curlParticleProfile.CurlNoiseType == CurlNoiseType.Normal ? false : true);

            _computeShader.SetFloat(_timeId, Time.time);
            _computeShader.SetFloat(_plumeBaseId, _curlParticleProfile.PlumeBase);
            _computeShader.SetFloat(_plumeHeightId, _curlParticleProfile.PlumeHeight);
            _computeShader.SetFloat(_plumeCeilingId, _curlParticleProfile.PlumeCeiling);
            _computeShader.SetFloat(_ringRadiusId, _curlParticleProfile.RingRadius);
            _computeShader.SetFloat(_ringMagnitudeId, _curlParticleProfile.RingMagnitude);
            _computeShader.SetFloat(_ringFalloffId, _curlParticleProfile.RingFalloff);
            _computeShader.SetFloat(_ringSpeedId, _curlParticleProfile.RingSpeed);
            _computeShader.SetFloat(_ringPerSecondId, _curlParticleProfile.RingPerSecond);
            _computeShader.SetFloat(_curlNoiseIntencityId, _curlParticleProfile.CurlNoiseIntencity);
            _computeShader.SetFloat(_speedFactorId, _curlParticleProfile.SpeedFactor);
            #endregion ### カールノイズパラメータ ###

            _computeShader.SetBuffer(_curlnoiseKernel, _particlesId, _particlesBuffer);
            _computeShader.SetBuffer(_curlnoiseKernel, _deadListId, _particlePoolBuffer);
            _computeShader.SetFloat(_deltaTimeId, Time.deltaTime);

            _computeShader.Dispatch(_curlnoiseKernel, _particleNumLimit / 8, 1, 1);

            _material.SetBuffer(_particlesId, _particlesBuffer);
            for (int i = 0; i < _meshNum; i++)
            {
                _propertyBlocks[i].SetFloat(_idOffsetId, _particleNumPerMesh * i);
                Graphics.DrawMesh(_combinedMesh, transform.position, transform.rotation, _material, 0, camera, 0, _propertyBlocks[i]);
            }
        }

        /// <summary>
        /// Initialize the particle.
        /// </summary>
        private void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;

            // Propety to id.
            {
                _useFakeId = Shader.PropertyToID("_UseFake");
                _octavesId = Shader.PropertyToID("_Octaves");
                _frequencyId = Shader.PropertyToID("_Frequency");
                _pId = Shader.PropertyToID("_P");
                _noiseScalesId = Shader.PropertyToID("_NoiseScales");
                _noiseGainId = Shader.PropertyToID("_NoiseGain");
                _risingForceId = Shader.PropertyToID("_RisingForce");
                _timeId = Shader.PropertyToID("_Time");
                _plumeBaseId = Shader.PropertyToID("_PlumeBase");
                _plumeHeightId = Shader.PropertyToID("_PlumeHeight");
                _plumeCeilingId = Shader.PropertyToID("_PlumeCeiling");
                _ringRadiusId = Shader.PropertyToID("_RingRadius");
                _ringMagnitudeId = Shader.PropertyToID("_RingMagnitude");
                _ringFalloffId = Shader.PropertyToID("_RingFalloff");
                _ringSpeedId = Shader.PropertyToID("_RingSpeed");
                _ringPerSecondId = Shader.PropertyToID("_RingPerSecond");
                _curlNoiseIntencityId = Shader.PropertyToID("_CurlNoiseIntencity");
                _speedFactorId = Shader.PropertyToID("_SpeedFactor");
                _particlesId = Shader.PropertyToID("_Particles");
                _deadListId = Shader.PropertyToID("_DeadList");
                _deltaTimeId = Shader.PropertyToID("_DeltaTime");
                _idOffsetId = Shader.PropertyToID("_IdOffset");

                _particlePoolId = Shader.PropertyToID("_ParticlePool");
                _positionId = Shader.PropertyToID("_Position");
                _colorId = Shader.PropertyToID("_Color");
                _minLifeTimeId = Shader.PropertyToID("_MinLifeTime");
                _maxLifeTimeId = Shader.PropertyToID("_MaxLifeTime");
                _delayId = Shader.PropertyToID("_Delay");
                _scaleId = Shader.PropertyToID("_Scale");
                _baseScaleId = Shader.PropertyToID("_BaseScale");
            }

            int seed = Mathf.Clamp(_curlParticleProfile.Seed, 0, 2 << 30 - 1);

            _p = CurlParticleUtility.CreateGrid(seed);

            _particles = GenerateParticles();

            // Generate combined mesh.
            {
                _particleNumPerMesh = CurlParticleUtility.MAX_VERTEX_NUM / _mesh.vertexCount;
                _meshNum = (int)Mathf.Ceil((float)_particleNumLimit / _particleNumPerMesh);

                _material = new Material(_shader);

                for (int i = 0; i < _meshNum; i++)
                {
                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    block.SetFloat(_idOffsetId, _particleNumPerMesh * i);
                    _propertyBlocks.Add(block);
                }

                _combinedMesh = CurlParticleUtility.CreateCombinedMesh(_mesh, _particleNumPerMesh);
            }

            // Generate ComputeBuffer
            {
                _particlesBuffer = new ComputeBuffer(_particleNumLimit, Marshal.SizeOf(typeof(Particle)), ComputeBufferType.Default);
                _particlePoolBuffer = new ComputeBuffer(_particleNumLimit, sizeof(int), ComputeBufferType.Append);
                _particlePoolBuffer.SetCounterValue(0);
                _particleArgsBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
                _particleArgs = new int[] { 0, 1, 0, 0 };
            }

            // Kernel info.
            {
                _initKernel = _computeShader.FindKernel("Init");
                _stopKernel = _computeShader.FindKernel("Stop");
                _emitKernel = _computeShader.FindKernel("Emit");
                _curlnoiseKernel = _computeShader.FindKernel("CurlNoiseMain");
            }

            DispatchInit();
        }

        /// <summary>
        /// パーティクルを生成する
        /// </summary>
        /// <returns></returns>
        private Particle[] GenerateParticles()
        {
            Particle[] particles = new Particle[_particleNumLimit];

            Vector3 colorVec = ColorVec;

            for (int i = 0; i < _particleNumLimit; i++)
            {
                Particle p = new Particle
                {
                    id = i,
                    active = 1,
                    position = Vector3.zero,
                    color = colorVec,
                    scale = 1.0f,
                    baseScale = _baseScale,
                    time = 0,
                    liefTime = 0,
                };

                particles[i] = p;
            }

            return particles;
        }

        /// <summary>
        /// Reset particles.
        /// </summary>
        /// <param name="paramList">Params for reseting.</param>
        /// <param name="particleCount">Count per param.</param>
        private void ResetParticles(ParticleParam[] paramList, int particleCount)
        {
            float maxLifeTime = float.MinValue;

            int pidx = 0;
            for (int i = 0; i < _particles.Length; i++)
            {
                pidx = i / particleCount;

                if (pidx < paramList.Length)
                {
                    _particles[i].active = 1;
                    _particles[i].position = CurlParticleUtility.GetRandomVector(paramList[pidx].Position, _randomRange);
                    _particles[i].delay = paramList[pidx].Delay;
                    _particles[i].color = paramList[pidx].Color;
                    _particles[i].scale = 1f;
                    _particles[i].time = 0;
                    _particles[i].liefTime = Random.Range(_minLifeTime, _maxLifeTime);
                    _particles[i].velocity = Vector3.zero;

                    float lifetime = _particles[i].liefTime + _particles[i].delay;
                    if (lifetime > maxLifeTime)
                    {
                        maxLifeTime = lifetime;
                    }
                }
                else
                {
                    _particles[i].active = 0;
                    _particles[i].liefTime = 0;
                }
            }

            _lifeTime = maxLifeTime;

            _particlesBuffer.SetData(_particles);
        }

        /// <summary>
        /// Compute Shaderで初期化処理
        /// </summary>
        private void DispatchInit()
        {
            _computeShader.SetBuffer(_initKernel, _particlesId, _particlesBuffer);
            _computeShader.SetBuffer(_initKernel, _deadListId, _particlePoolBuffer);
            _computeShader.Dispatch(_initKernel, _particleNumLimit / 8, 1, 1);
        }

        /// <summary>
        /// Stop the particles.
        /// </summary>
        private void DispatchStop()
        {
            _computeShader.SetBuffer(_stopKernel, _particlesId, _particlesBuffer);
            _computeShader.SetBuffer(_stopKernel, _deadListId, _particlePoolBuffer);
            _computeShader.Dispatch(_stopKernel, _particleNumLimit / 8, 1, 1);
        }

        /// <summary>
        /// Emit with vertices (List ver).
        /// </summary>
        /// <param name="vertices">List of vertices.</param>
        /// <param name="particleCount">Particle count per vert.</param>
        public void EmitWithVertices(List<Vector3> vertices, int particleCount, float delay = 0)
        {
            EmitWithVertices(vertices.ToArray(), particleCount, delay);
        }

        /// <summary>
        /// 指定した頂点位置にパーティクルを生成する
        /// </summary>
        /// <param name="vertices">頂点郡</param>
        /// <param name="particleCount">頂点ごとのパーティクル数</param>
        public void EmitWithVertices(Vector3[] vertices, int particleCount, float delay = 0)
        {
            ParticleParam[] paramList = new ParticleParam[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                ParticleParam p = DefaultParam;
                p.Position = vertices[i];
                p.Delay = delay;
                paramList[i] = p;
            }

            Burst(paramList, particleCount);
        }

        /// <summary>
        /// パーティクルの生成位置に利用するメッシュターゲット
        /// </summary>
        /// <param name="target">メッシュターゲット</param>
        /// <param name="particleCount">頂点ごとのパーティクル数</param>
        public void EmitWithMesh(MeshFilter filter, int particleCount, float delay = 0)
        {
            Vector3[] vertices = filter.mesh.vertices;

            Matrix4x4 m = filter.transform.localToWorldMatrix;

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = m.MultiplyPoint(CurlParticleUtility.GetRandomVector(vertices[i], _randomRange));
            }

            EmitWithVertices(vertices, particleCount, delay);
        }

        /// <summary>
        /// パーティクルをエミットする
        /// </summary>
        /// <param name="p">パーティクル設定</param>
        /// <param name="count">パーティクル数</param>
        public void Emit(ParticleParam p, int count)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            _computeShader.SetBuffer(_emitKernel, _particlesId, _particlesBuffer);
            _computeShader.SetBuffer(_emitKernel, _particlePoolId, _particlePoolBuffer);
            _computeShader.SetVector(_positionId, CurlParticleUtility.GetRandomVector(p.Position, _randomRange));
            _computeShader.SetVector(_colorId, p.Color);
            _computeShader.SetFloat(_minLifeTimeId, _minLifeTime);
            _computeShader.SetFloat(_maxLifeTimeId, _maxLifeTime);
            _computeShader.SetFloat(_delayId, p.Delay);
            _computeShader.SetFloat(_scaleId, 1.0f);
            _computeShader.SetFloat(_baseScaleId, _baseScale);

            int poolSize = GetParticlePoolSize() / 8;

            if (poolSize <= 0)
            {
                return;
            }

            int countSize = count / 8;
            int size = Mathf.Min(countSize, poolSize);

            _computeShader.Dispatch(_emitKernel, size, 1, 1);

            float lifeTime = _maxLifeTime + p.Delay;
            if (lifeTime > _lifeTime)
            {
                _lifeTime = lifeTime;
            }

            Activate();
        }

        /// <summary>
        /// Set particles' params and emit.
        /// </summary>
        /// <param name="paramList">Burst emit particle's list.</param>
        /// <param name="particleCount">Particle count per param.</param>
        public void Burst(ParticleParam[] paramList, int particleCount)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            DispatchStop();

            ResetParticles(paramList, particleCount);

            Activate();
        }

        /// <summary>
        /// バッファなどを破棄する
        /// </summary>
        private void Dispose()
        {
            if (_particlesBuffer != null)
            {
                _particlesBuffer.Release();
            }

            if (_particleArgsBuffer != null)
            {
                _particleArgsBuffer.Release();
            }

            if (_particlePoolBuffer != null)
            {
                _particlePoolBuffer.Release();
            }

            if (_gridData != null)
            {
                _gridData.Release();
            }

            _particlesBuffer = null;
            _particlePoolBuffer = null;
            _particleArgsBuffer = null;
            _gridData = null;

            _isActive = false;
        }
    }
}
