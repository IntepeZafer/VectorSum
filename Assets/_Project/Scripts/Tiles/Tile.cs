using UnityEngine;
using TMPro;
using Game.Grid;
using Game.Core;

namespace Game.Tiles
{
    public class Tile : MonoBehaviour
    {
        public int Value {get; private set;}
        public MoveDirection AllowedDirections {get; private set;}
        public Cell CurrentCell {get; private set;}

        [Header("Visuals")]
        [SerializeField] private TMP_Text valueText;
        [SerializeField] private SpriteRenderer backgroundRenderer;
        
        [Header("Direction Icons")]
        [SerializeField] private GameObject upArrow;
        [SerializeField] private GameObject downArrow;
        [SerializeField] private GameObject leftArrow;
        [SerializeField] private GameObject rightArrow;

        public void Initialize(int value , MoveDirection directions , Cell cell)
        {
            Value = value;
            AllowedDirections = directions;
            SetCell(cell);
            UpdateVisuals();
        }
        public void SetCell(Cell cell)
        {
            if(CurrentCell != null) CurrentCell.CurrentTile = null; // Eski Hücreyi Boşalt
            CurrentCell = cell; // Yeni Hücreye Yerleş
            CurrentCell.CurrentTile = this;
            transform.position = cell.transform.position; // Görsel Olarak Hücrenin Pozisyonuna Git
        }
        private void UpdateVisuals()
        {
            valueText.text = Value.ToString(); // İleride Buraya Yön İkonları Ve Sayıya Göre Renk Değişimi Eklenecek
            // Yön oklarını AllowedDirections'a göre aç/kapat
            // HasFlag kontrolü ile hangi yönlerin aktif olduğunu anlıyoruz
            upArrow.SetActive(AllowedDirections.HasFlag(MoveDirection.Up));
            downArrow.SetActive(AllowedDirections.HasFlag(MoveDirection.Down));
            leftArrow.SetActive(AllowedDirections.HasFlag(MoveDirection.Left));
            rightArrow.SetActive(AllowedDirections.HasFlag(MoveDirection.Right));
        }
    }
}