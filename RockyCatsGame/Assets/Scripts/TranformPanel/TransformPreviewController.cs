using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TransformPreviewController : MonoBehaviour
{
    // ======  INVENTARIO (HARDCODE)  ======
    [Header("Coins iniciales (hardcoded)")]
    public int coinsWater = 4;
    public int coinsTemp = 2;
    public int coinsPressure = 1;
    public int coinsTime = 3;

    // ======  STATS (0..5)  ======
    [Header("Stats actuales (0..5)")]
    [Range(0, 5)] public int statWater = 3;
    [Range(0, 5)] public int statTemp = 4;
    [Range(0, 5)] public int statPressure = 1;
    [Range(0, 5)] public int statTime = 1;

    // Cambios pendientes (Add/Take). Se resetean a 0 tras Transform.
    int dWater, dTemp, dPressure, dTime;

    [System.Serializable]
    public class StatUI
    {
        public Image bar;          // WaterBarImg / TempBarImg / ...
        public TMP_Text deltaText; // AddOrTake*/Text (TMP)
        public Button addBtn;      // AddOrTake*/AddBtn
        public Button takeBtn;     // AddOrTake*/TakeBtn
    }

    [Header("UI de Stats (barras + add/take + delta)")]
    public StatUI water;
    public StatUI temp;
    public StatUI pressure;
    public StatUI time;

    [Header("UI del inventario (derecha del personaje)")]
    public TMP_Text waterCountText;    // InventoryUI/CoinsWater/Count(TMP)
    public TMP_Text tempCountText;     // InventoryUI/CoinsTemp/Count(TMP)
    public TMP_Text pressureCountText; // InventoryUI/CoinsPressure/Count(TMP)
    public TMP_Text timeCountText;     // InventoryUI/CoinsTime/Count(TMP)

    [Header("Bot�n Aplicar Transformaci�n")]
    public Button transformBtn;

    // ====== Reglas de transformaci�n ======
    [System.Serializable]
    public struct RockRecipe
    {
        public RockState state;                 // Magma / Ignea / Sedimento
        [Range(0, 5)] public int water, temp, pressure, time;
    }

    [Header("Recetas v�lidas (puedes editarlas en el Inspector)")]
    public RockRecipe[] recipes;

    [Header("Feedback y cierre")]
    public TMP_Text feedbackText;     // mensaje de error si no coincide
    public GameObject overlayToClose; // tu OverlayRoot/StatusPanel para cerrarlo

    // ====== Salida para tu compañero ======
    [Header("Salida: resultado de la �ltima transformaci�n")]
    public bool newRockStateReady;
    public RockState newRockState;

    // ---- Defaults de recetas ----
    RockRecipe[] DefaultRecipes() => new RockRecipe[] {
        new RockRecipe { state = RockState.Magma,     water = 0, temp = 5, pressure = 3, time = 1 },
        new RockRecipe { state = RockState.Ignea,     water = 0, temp = 3, pressure = 5, time = 2 },
        new RockRecipe { state = RockState.Sedimento, water = 5, temp = 0, pressure = 2, time = 4 },
    };

    // Se llama al a�adir el componente o si haces "Reset" en el Inspector
    void Reset()
    {
        recipes = DefaultRecipes();
    }

    void Awake()
    {
        if (recipes == null || recipes.Length == 0)
            recipes = DefaultRecipes();

        // Asegura barras tipo Filled (horizontal)
        SetupFilled(water.bar);
        SetupFilled(temp.bar);
        SetupFilled(pressure.bar);
        SetupFilled(time.bar);

        // Hooks de botones (usar monedas en ambas direcciones)
        if (water.addBtn) water.addBtn.onClick.AddListener(() => { TryAdd(ref dWater, statWater, coinsWater); RefreshUI(); });
        if (water.takeBtn) water.takeBtn.onClick.AddListener(() => { TryTake(ref dWater, statWater, coinsWater); RefreshUI(); });

        if (temp.addBtn) temp.addBtn.onClick.AddListener(() => { TryAdd(ref dTemp, statTemp, coinsTemp); RefreshUI(); });
        if (temp.takeBtn) temp.takeBtn.onClick.AddListener(() => { TryTake(ref dTemp, statTemp, coinsTemp); RefreshUI(); });

        if (pressure.addBtn) pressure.addBtn.onClick.AddListener(() => { TryAdd(ref dPressure, statPressure, coinsPressure); RefreshUI(); });
        if (pressure.takeBtn) pressure.takeBtn.onClick.AddListener(() => { TryTake(ref dPressure, statPressure, coinsPressure); RefreshUI(); });

        if (time.addBtn) time.addBtn.onClick.AddListener(() => { TryAdd(ref dTime, statTime, coinsTime); RefreshUI(); });
        if (time.takeBtn) time.takeBtn.onClick.AddListener(() => { TryTake(ref dTime, statTime, coinsTime); RefreshUI(); });

        if (transformBtn) transformBtn.onClick.AddListener(ApplyTransform);

        RefreshUI();
    }

    void SetupFilled(Image img)
    {
        if (!img) return;
        img.type = Image.Type.Filled;
        img.fillMethod = Image.FillMethod.Horizontal;
        img.fillOrigin = 0;
    }

    // ---- Add/Take con monedas ----
    void TryAdd(ref int delta, int baseStat, int baseCoins)
    {
        if (baseStat + delta >= 5) return;
        if (delta >= 0)
        {
            int coinsAfterThisStep = baseCoins - Mathf.Abs(delta);
            if (coinsAfterThisStep <= 0) return;
        }
        delta += 1;
    }

    void TryTake(ref int delta, int baseStat, int baseCoins)
    {
        if (baseStat + delta <= 0) return;
        if (delta <= 0)
        {
            int coinsAfterThisStep = baseCoins - Mathf.Abs(delta);
            if (coinsAfterThisStep <= 0) return;
        }
        delta -= 1;
    }

    // ---- UI ----
    void RefreshUI()
    {
        UpdateStatUI(water, statWater, dWater);
        UpdateStatUI(temp, statTemp, dTemp);
        UpdateStatUI(pressure, statPressure, dPressure);
        UpdateStatUI(time, statTime, dTime);

        // Inventario mostrado = coins - |delta|
        if (waterCountText) waterCountText.text = Mathf.Max(0, coinsWater - Mathf.Abs(dWater)).ToString();
        if (tempCountText) tempCountText.text = Mathf.Max(0, coinsTemp - Mathf.Abs(dTemp)).ToString();
        if (pressureCountText) pressureCountText.text = Mathf.Max(0, coinsPressure - Mathf.Abs(dPressure)).ToString();
        if (timeCountText) timeCountText.text = Mathf.Max(0, coinsTime - Mathf.Abs(dTime)).ToString();

        bool CanAdd(int s, int d, int c) => (s + d < 5) && (d < 0 || (c - Mathf.Abs(d) > 0));
        bool CanTake(int s, int d, int c) => (s + d > 0) && (d > 0 || (c - Mathf.Abs(d) > 0));

        if (water.addBtn) water.addBtn.interactable = CanAdd(statWater, dWater, coinsWater);
        if (water.takeBtn) water.takeBtn.interactable = CanTake(statWater, dWater, coinsWater);
        if (temp.addBtn) temp.addBtn.interactable = CanAdd(statTemp, dTemp, coinsTemp);
        if (temp.takeBtn) temp.takeBtn.interactable = CanTake(statTemp, dTemp, coinsTemp);
        if (pressure.addBtn) pressure.addBtn.interactable = CanAdd(statPressure, dPressure, coinsPressure);
        if (pressure.takeBtn) pressure.takeBtn.interactable = CanTake(statPressure, dPressure, coinsPressure);
        if (time.addBtn) time.addBtn.interactable = CanAdd(statTime, dTime, coinsTime);
        if (time.takeBtn) time.takeBtn.interactable = CanTake(statTime, dTime, coinsTime);

        if (feedbackText) feedbackText.text = "";
        if (transformBtn) transformBtn.interactable =
            (dWater != 0 || dTemp != 0 || dPressure != 0 || dTime != 0);
    }

    void UpdateStatUI(StatUI ui, int baseStat, int delta)
    {
        int preview = Mathf.Clamp(baseStat + delta, 0, 5);
        if (ui.bar) ui.bar.fillAmount = preview / 5f;

        if (ui.deltaText)
        {
            ui.deltaText.text = (delta >= 0 ? "+" : "") + delta.ToString();
            if (delta == 0) ui.deltaText.color = Color.white;
            else if (delta > 0) ui.deltaText.color = new Color(0.18f, 0.7f, 0.2f);
            else ui.deltaText.color = new Color(0.85f, 0.3f, 0.25f);
        }
    }

    // ---- Aplicar ----
    void ApplyTransform()
    {
        int fw = Mathf.Clamp(statWater + dWater, 0, 5);
        int ft = Mathf.Clamp(statTemp + dTemp, 0, 5);
        int fp = Mathf.Clamp(statPressure + dPressure, 0, 5);
        int fti = Mathf.Clamp(statTime + dTime, 0, 5);

        if (!TryMatchRecipe(fw, ft, fp, fti, out RockState matched))
        {
            if (feedbackText) feedbackText.text = "Debes poner los valores exactos de una roca.";
            return;
        }

        Commit(ref statWater, ref coinsWater, ref dWater);
        Commit(ref statTemp, ref coinsTemp, ref dTemp);
        Commit(ref statPressure, ref coinsPressure, ref dPressure);
        Commit(ref statTime, ref coinsTime, ref dTime);

        newRockStateReady = true;
        newRockState = matched;

        if (overlayToClose) overlayToClose.SetActive(false);

        RefreshUI();
    }

    bool TryMatchRecipe(int w, int t, int p, int ti, out RockState state)
    {
        for (int i = 0; i < recipes.Length; i++)
        {
            var r = recipes[i];
            if (r.water == w && r.temp == t && r.pressure == p && r.time == ti)
            {
                state = r.state;
                return true;
            }
        }
        state = default;
        return false;
    }

    void Commit(ref int stat, ref int coins, ref int delta)
    {
        int newStat = Mathf.Clamp(stat + delta, 0, 5);
        int applied = Mathf.Abs(newStat - stat);     // magnitud del cambio
        coins = Mathf.Max(0, coins - applied);       // SIEMPRE gasta monedas
        stat = newStat;
        delta = 0;                                   // resetea a 0
    }
}
