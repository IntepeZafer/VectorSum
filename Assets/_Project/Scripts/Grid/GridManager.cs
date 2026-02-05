using System.Collections.Generic;
using Game.Core;
using Game.Tiles;
using UnityEngine;
using System.Linq;

namespace Game.Grid
{
    public class GridManager : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private int width = 4; // Izgara Genişliği
        [SerializeField] private int height = 4; // Izgara Yüksekliği
        [SerializeField] private float spacing = 1.1f; // Hücreler Arası Mesafe

        [Header("References")]
        [SerializeField] private Cell cellPrefab; // Oluşturulacak Hücre Prefabı
        [SerializeField] private Transform gridParent; // Hücrelerin Hiyerarşide Duracağı Yer

        [Header("Tile Settings")]
        [SerializeField] private Tile tilePrefab;

        private Dictionary<Vector2Int , Cell> _cell = new Dictionary<Vector2Int, Cell>(); // Koordinatlara Göre Hücrelere Hızlıca Ulaşmak İçin Bir Sözlük (Dictionary)

        void Start()
        {
            GenerateGrid(); // Izgarayı oluşturur
            SpawnTile();    // 1. taşı oluşturur
            SpawnTile();    // 2. taşı oluşturur
        }
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) PrepareMovement(Vector2Int.up);
            else if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) PrepareMovement(Vector2Int.down);
            else if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) PrepareMovement(Vector2Int.left);
            else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) PrepareMovement(Vector2Int.right);

        }
        public void GenerateGrid()
        {
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    Vector3 pos = new Vector3(x * spacing , y * spacing , 0); // Pozisyonu Hesapla (X Ve Y) Koordinatlarını Spacing İle Çarpıyoruz
                    Cell spawnedCell = Instantiate(cellPrefab , pos , Quaternion.identity , gridParent); // Hücreyi Oluştur
                    // Hücreyi Koordinatıyla Başlat
                    Vector2Int coords = new Vector2Int(x , y);
                    spawnedCell.Initialize(coords);
                    spawnedCell.name = $"Cell : {x}_{y}";
                    _cell.Add(coords , spawnedCell); // Sözlüğe Ekle (İleride Hücreyi Koordinatla Çağırmak İçin)
                }
            }
            CenterCamera();
        }
        private void CenterCamera()
        {
            // Kamerayı Izgaranın Tam Ortasına Hizalar
            float centerX = (width - 1) * spacing / 2f;
            float centerY = (height - 1) * spacing / 2f;
            Camera.main.transform.position = new Vector3(centerX , centerY , -10); 
        }
        public List<Cell> GetEmptyCells() // Boş hücreleri listeleyen yardımcı fonksiyon
        {
            List<Cell> emptyCells = new List<Cell>();
            foreach(var cell in _cell.Values)
            {
                if(!cell.IsOccupied) emptyCells.Add(cell);
            }
            return emptyCells;
        }
        public void SpawnTile()
        {
            var emptyCells = GetEmptyCells();
            if(emptyCells.Count == 0) return;
            Cell randomCell = emptyCells[Random.Range(0 , emptyCells.Count)]; // Rastgele bir boş hücre seç
            Tile spawnedTile = Instantiate(tilePrefab , randomCell.transform.position , Quaternion.identity , gridParent); // Taşı Oluştur
            //spawnedTile.Initialize(2 , MoveDirection.All , randomCell); // Şimdilik Test İçin : Değer 2 Yön Hepsi (All)
            MoveDirection[] possibleDirections =
            {
                MoveDirection.All,
                MoveDirection.Horizontal,
                MoveDirection.Vertical,
                MoveDirection.Up,
                MoveDirection.Down
            };
            MoveDirection randomDir = possibleDirections[Random.Range(0 , possibleDirections.Length)];
            spawnedTile.Initialize(2 , randomDir , randomCell);
        }
        public void PrepareMovement(Vector2Int direction)
        {
            // Hücreleri hareket yönüne göre sıralıyoruz
            // Örn: Sağa gidiliyorsa, X değeri en büyük (3) olandan başlıyoruz.
            var orderedCells = _cell.Values.OrderByDescending(Cell => Cell.Coordinates.x * direction.x + Cell.Coordinates.y * direction.y).ToList();
            foreach(var cell in orderedCells)
            {
                if (cell.IsOccupied)
                {
                    MoveTile(cell.CurrentTile , direction);
                }
            }
            SpawnTile(); // Hareket bittikten sonra yeni bir taş doğur
        }
        private void MoveTile(Tile tile , Vector2Int direction)
        {
            MoveDirection moveDirEnum = GetDirectionEnum(direction); // 1. KURAL: Taşın bu yöne gitme izni var mı?
            if (!tile.AllowedDirections.HasFlag(moveDirEnum))
            {
                return; // İzni yoksa hareket etme
            }
            Cell currentCell = tile.CurrentCell;
            Cell targetCell = null;
            // Gidilebilecek en uzak boş hücreyi bul
            Vector2Int nextCoords = currentCell.Coordinates + direction; // Gidilebilecek en uzak boş hücreyi bul
            while (true)
            {
                Cell nextCell = GetCellAt(nextCoords);
                if(nextCell != null && !nextCell.IsOccupied)
                {
                    targetCell = nextCell;
                    nextCoords += direction; // Bir adım daha ileri bak
                }
                else
                {
                    break; // Ya duvar var ya da dolu bir hücre
                }
            }
            if(targetCell != null)
            {
                tile.SetCell(targetCell);
            }
        }
        // Vector2Int yönünü bizim Enum tipine çeviren yardımcı fonksiyon
        private MoveDirection GetDirectionEnum(Vector2Int direction)
        {
            if(direction == Vector2Int.up) return MoveDirection.Up;
            if(direction == Vector2Int.down) return MoveDirection.Down;
            if(direction == Vector2Int.left) return MoveDirection.Left;
            if(direction == Vector2Int.right) return MoveDirection.Right;
            return MoveDirection.None;
        }
        public Cell GetCellAt(Vector2Int coords)
        {
            if(_cell.TryGetValue(coords , out var cell)) return cell;
            return null;
        }
    }
}
