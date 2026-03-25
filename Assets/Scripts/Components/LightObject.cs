using UnityEngine;
namespace Assets.Scripts.Components
{
    public class LightObject : ObjectBase
    {
        private Light _light { get; set; }

        [field: SerializeField]
        private float _intensity { get; set; }

        [field: SerializeField]
        private float _range { get; set; }

        [field: SerializeField]
        private Color _color { get; set; }

        [field: SerializeField]
        private LightShadows _shadowType { get; set; }

        [field: SerializeField]
        private float _shadowStrength { get; set; }

        [field: SerializeField]
        [HideInInspector]
        private LightType _lightType { get; set; }

        [field: SerializeField]
        private LightShape _lightShape { get; set; }
        
        [field: SerializeField]
        private float _spotAngle { get; set; }

        [field: SerializeField]
        private float _innerSpotAngle { get; set; }

        public float Intensity
        { 
            get => _intensity;
            set
            {
                _intensity = value; 
                if (_light != null) 
                    _light.intensity = value;
            } 
        }

        public float Range
        {
            get => _range;
            set
            {
                _range = value;
                if (_light != null)
                    _light.range = value;
            }
        }

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                if (_light != null)
                    _light.color = value;
            }
        }

        public LightShadows ShadowType
        {
            get => _shadowType;
            set
            {
                _shadowType = value;
                if (_light != null)
                    _light.shadows = value;
            }
        }

        public float ShadowStrength
        {
            get => _shadowStrength;
            set
            {
                _shadowStrength = value;
                if (_light != null)
                    _light.shadowStrength = value;
            }
        }

        public LightType LightType
        {
            get => _lightType;
            set
            {
                _lightType = value;
                if (_light != null)
                    _light.type = value;
            }
        }

        public LightShape LightShape
        {
            get => _lightShape;
            set
            {
                _lightShape = value;
                if (_light != null)
                    _light.shape = value;
            }
        }

        public float SpotAngle
        {
            get => _spotAngle;
            set
            {
                _spotAngle = value;
                if (_light != null)
                    _light.spotAngle = value;
            }
        }

        public float InnerSpotAngle
        {
            get => _innerSpotAngle;
            set
            {
                _innerSpotAngle = value;
                if (_light != null)
                    _light.innerSpotAngle = value;
            }
        }

        private void Awake()
        {
            _light = gameObject.GetComponent<Light>() ?? gameObject.AddComponent<Light>();
            ApplyLight();
        }

        private void OnValidate()
        {
            if (_light == null)
                _light = gameObject.GetComponent<Light>() ?? gameObject.AddComponent<Light>();

            ApplyLight();
        }

        private void ApplyLight()
        {
            _light.type = _lightType;
            _light.color = _color;
            _light.intensity = _intensity;
            _light.range = _range;
            _light.shadows = _shadowType;
            _light.shadowStrength = _shadowStrength;
            _light.shape = _lightShape;

            switch (_lightType)
            {
                case LightType.Spot:
                    _light.spotAngle = _spotAngle;
                    _light.innerSpotAngle = _innerSpotAngle;
                    break;

                case LightType.Rectangle:
                case LightType.Disc:
                    _light.areaSize = new Vector2(_range, _range);
                    break;
            }
        }

        public override void Compile(Transform root)
        {
            base.Compile(root);
            base.Properties = new()
            {
                ["LightIntensity"] = _intensity,
                ["LightRange"] = _range,
                ["LightColor"] = _color,
                ["ShadowType"] = _shadowType,
                ["ShadowStrength"] = _shadowStrength,
                ["LightType"] = _lightType,
                ["LightShape"] = _lightShape,
                ["SpotAngle"] = _spotAngle,
                ["InnerSpotAngle"] = _innerSpotAngle
            };
        }
    }
}
