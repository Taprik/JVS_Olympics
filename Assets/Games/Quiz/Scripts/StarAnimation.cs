using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StarAnimation : CustomAnimation
{
    [SerializeField] protected RectTransform _star;
    protected List<RectTransform> _stars = new List<RectTransform>();
    [SerializeField] protected float _timeStep;
    [SerializeField] private float _jumpLength;
    [SerializeField] protected float _scaleMax;
    [SerializeField] protected int _maxStar;
    [SerializeField] protected int _travelStep;
    [SerializeField] private float _perStepAngle;
    // Start is called before the first frame update
    public override void Start()
    {
        StartCoroutine(Animate());
    }

    private void OnDisable()
    {
        while (_stars.Count > 0)
        {
            _stars.RemoveAt(0);
        }
        Destroy(gameObject);
    }
    public override void SetColor(Color color) => _color = color;
    public override IEnumerator Animate()
    {
        Color currentColor = _color;
        RectTransform currentStar;
        for (int i = 1; i<= 3; i++) 
        {
            currentStar =  Instantiate(_star, transform);
            Vector3 currentScale;

            currentStar.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * _scaleMax, i / 3f);
            _stars.Add(currentStar);
            Image image = currentStar.GetComponent<Image>();
            if (image == null)
                image = currentStar.GetComponentInChildren<Image>();

            image.color = currentColor;
            yield return new WaitForSeconds(_timeStep );
        }
        
        float rng = Random.Range(0f, Mathf.PI*2);
        if (rng < Mathf.PI * 0.5f || rng > Mathf.PI * 1.5f)
            _perStepAngle *= -1;
        float x = Mathf.Cos(rng);
        float y = Mathf.Sin(rng);
        float fall = 0.2f;
        Vector2 direction = new Vector2(x, y);

        for(int i = 0;i< _travelStep; i++) 
        {
            currentStar = Instantiate(_star, transform);
            currentStar.eulerAngles = new Vector3(0, 0, _perStepAngle * i);
            currentStar.anchoredPosition = _stars[_stars.Count - 1].anchoredPosition + direction * _jumpLength;
            currentStar.localScale = Vector3.one * _scaleMax;

            _stars.Add(currentStar);
            if(_stars.Count > _maxStar)
            {
                Destroy(_stars[0].gameObject);
                _stars.RemoveAt(0);
            }
            Fade(currentColor);
            yield return new WaitForSeconds(_timeStep);
            direction.y -= fall;
            direction= direction.normalized;    
            
        }
        while (_stars.Count > 0)
        {
            Destroy(_stars[0].gameObject);
            _stars.RemoveAt(0);
            Fade(currentColor);
            yield return new WaitForSeconds(_timeStep);
        }
        Destroy(gameObject);

    } 
    protected void Fade(Color currentColor) 
    {
        for (float j = 0; _stars.Count > j; j++)
        {
            Image image = _stars[(int)j].GetComponent<Image>();
            if (image == null)
                image = _stars[(int)j].GetComponentInChildren<Image>();
            Color color = currentColor;
            color.a = Mathf.Lerp(0, 1, (j) / _stars.Count);
            image.color = color;
        }
    }

    public Color GetColor() => _color;
}
