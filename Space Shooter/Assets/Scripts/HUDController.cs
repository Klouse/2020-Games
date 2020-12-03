using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using System;

public class HUDController : MonoBehaviour
{
    [SerializeField]
    GameManager _gameManager;
    private float _masterAngleEnd = 360f;
    [SerializeField]
    private float _healthTotal = 3f;
    [SerializeField]
    private float _currentHealth;
    [SerializeField]
    private float _currentShieldTimer = 0f;
    private float _currentShieldDelaySeconds = 0f;
    private float _startingShieldDelaySeconds;
    private float _hudXValue = -2f;
    private float _healthArcMinAngle = 7f;
    private Dictionary<int, Color> _hudColors = new Dictionary<int, Color>();
    [SerializeField]
    Color healthArcStartColor = new Color(100f, 0f, 0f, 255f);
    [SerializeField]
    Color healthArcEndColor = new Color(255f, 0f, 0f, 255f);
    [SerializeField]
    Color negativeArcStartColor = new Color(0f, 0f, 100f, 255f);
    [SerializeField]
    Color negativeArcEndColor = new Color(0f, 0f, 255f, 255f);
    [SerializeField]
    Color shieldStartColor = new Color(0f, 0f, 255f, 255f);
    [SerializeField]
    Color shieldEndColor = new Color(0f, 0f, 255f, 255f);

    private void Start() {
        _currentHealth = _healthTotal;
        _hudColors.Add(0, healthArcStartColor); // HealthArcStart
        _hudColors.Add(1, healthArcEndColor); // HealthArcEnd
        _hudColors.Add(2, negativeArcStartColor); // NegativeArcStart
        _hudColors.Add(3, negativeArcEndColor); // NegativeArcEnd
        _hudColors.Add(4, shieldStartColor); // Start Shield Color
        _hudColors.Add(5, shieldEndColor); // End Shield Color
    }
    
    private void Update() {
        if (_currentShieldDelaySeconds > 0f){
            _currentShieldTimer = _currentShieldDelaySeconds / _startingShieldDelaySeconds * 360f;
            _currentShieldDelaySeconds -= Time.deltaTime;
        }else{
            _currentShieldTimer = 0f;
            _currentShieldDelaySeconds = 0f;
        }
    }

    public void SetHealthTotal(int maxHealth){
        _healthTotal = maxHealth;
    }

    public void SetCurrentHealth(int health, int totalHealth){
        _healthTotal = totalHealth;
        _currentHealth  = health;
    }

   void OnPostRender() {
       if (_gameManager.GameStarted && _gameManager.GameOver == false){
        DrawPositiveHealthArc();
        DrawNegativeHealthArc();
        DrawShieldPie();
       }
    }

    void DrawPositiveHealthArc(){
        float startAngle = (_masterAngleEnd - ((_currentHealth / _healthTotal) * _masterAngleEnd)) + _healthArcMinAngle;
        if (startAngle > _masterAngleEnd){
            startAngle = _masterAngleEnd;
        }
        float angleStart = Mathf.Deg2Rad * startAngle;
        float angleEnd = Mathf.Deg2Rad * _masterAngleEnd;
        float healthArcNumberOfDashes = _currentHealth;
        float healthArcDashSpacing = 0.07f;
        float healthArcRadius = 0.35f;
        float healthArcThickness = 0.15f;
        Vector3 healthArcLocation = new Vector3(_hudXValue, 5.2f, 0f);
        DashStyle healthArcDashStyle = new DashStyle(healthArcNumberOfDashes, healthArcDashSpacing, DashType.Rounded);
        healthArcDashStyle.space = DashSpace.FixedCount;
        healthArcDashStyle.snap = DashSnapping.EndToEnd;
        Draw.ArcGradientAngularDashed(healthArcLocation,healthArcDashStyle,healthArcRadius,healthArcThickness, angleStart, angleEnd, ArcEndCap.None, _hudColors[0], _hudColors[1]);
    }

    void DrawNegativeHealthArc(){
        float negativeAngleStart = Mathf.Deg2Rad * _healthArcMinAngle;
        float endAngle = (_masterAngleEnd - ((_currentHealth / _healthTotal) * _masterAngleEnd));
        if (endAngle < _healthArcMinAngle){
            endAngle = _healthArcMinAngle;
        }
        float negativeAngleEnd = Mathf.Deg2Rad *  endAngle;
        float negativeArcNumberOfDashes = _healthTotal - _currentHealth;
        float negativeArcDashSpacing = 0.07f;
        float negativeArcRadius = 0.35f;
        float negativeArcThickness = 0.16f;
        Vector3 negativeArcLocation = new Vector3(_hudXValue, 5.2f, 0f);
        DashStyle negativeArcDashStyle = new DashStyle(negativeArcNumberOfDashes, negativeArcDashSpacing, DashType.Rounded);
        negativeArcDashStyle.space = DashSpace.FixedCount;
        negativeArcDashStyle.snap = DashSnapping.EndToEnd;
        Draw.ArcGradientAngularDashed(negativeArcLocation,negativeArcDashStyle,negativeArcRadius,negativeArcThickness, negativeAngleStart, negativeAngleEnd, ArcEndCap.None, _hudColors[2], _hudColors[3]);
    }

    void DrawShieldPie(){
        float offset = 90f;
        float AngleStart = (Mathf.Deg2Rad * (0f + offset));
        //float AngleStart = 0f;
       float AngleEnd = (Mathf.Deg2Rad * (_currentShieldTimer + offset));
       //float AngleEnd = 0f;
       float pieRadius = 0.35f;
       Vector3 negativeArcLocation = new Vector3(_hudXValue, 4.2f, 0f);
       Color startingColor = new Color(0f, 13f, 255f, 255f);
       Color endColor = new Color(0f, 169f, 233f, 255f);
       Draw.PieGradientRadial(negativeArcLocation, pieRadius, AngleStart, AngleEnd,_hudColors[4], _hudColors[5]);
    }

    internal void SetShieldDelay(float seconds)
    {
        _currentShieldDelaySeconds = seconds;
        _startingShieldDelaySeconds = seconds;
    }
}
