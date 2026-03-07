#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Genera la escena del puzzle "El Orden Invertido" automáticamente.
/// Menú: Tools > Puzzle > Generar Puzzle 4 Jugadores
///
/// MAPA generado:
///
///   ╔══════════════════════════╗
///   ║  [P1🔵]      [P2🔴]     ║
///   ║                          ║
///   ║  [L3🟢]  ✦  [L4🟡]    ║   L = palanca
///   ║         KEY              ║
///   ║  [L1🔵]     [L2🔴]     ║   Regla: P1↔L3, P2↔L4, P3↔L1, P4↔L2
///   ║                          ║
///   ║  [P3🟢]      [P4🟡]    ║
///   ║                          ║
///   ║        [SALIDA]          ║
///   ╚══════════════════════════╝
///
/// Cada jugador empieza en su esquina, pero la palanca que puede activar
/// está en la esquina OPUESTA — debe cruzar el mapa.
/// </summary>
public class PuzzleSceneGenerator : MonoBehaviour
{
    [MenuItem("Tools/Puzzle/Generar Puzzle 4 Jugadores")]
    static void Generate()
    {
        // ── Colores de jugadores ──────────────────────────────────────────
        Color[] colors = {
            new Color(0.25f, 0.55f, 1f),      // P1 azul
            new Color(1f,    0.3f,  0.3f),    // P2 rojo
            new Color(0.25f, 0.9f,  0.35f),   // P3 verde
            new Color(1f,    0.85f, 0.1f),    // P4 amarillo
        };

        // Esquemas de control
        var controls = new PuzzlePlayer4.ControlScheme[]
        {
            PuzzlePlayer4.ControlScheme.WASD,
            PuzzlePlayer4.ControlScheme.Arrows,
            PuzzlePlayer4.ControlScheme.IJKL,
            PuzzlePlayer4.ControlScheme.Numpad,
        };

        // ── Posiciones ────────────────────────────────────────────────────
        // Jugadores: esquinas exteriores
        Vector3[] playerPos = {
            new(-4f,  4f, 0),   // P1 arriba-izq
            new( 4f,  4f, 0),   // P2 arriba-der
            new(-4f, -4f, 0),   // P3 abajo-izq
            new( 4f, -4f, 0),   // P4 abajo-der
        };

        // Palancas: esquinas OPUESTAS a sus dueños
        // Regla: P1(idx0) activa L que allowedIndex=0, colocada en esquina de P3
        Vector3[] leverPos = {
            new(-3f, -2.5f, 0),  // L para P1 → esquina de P3 (abajo-izq)
            new( 3f, -2.5f, 0),  // L para P2 → esquina de P4 (abajo-der)
            new(-3f,  2.5f, 0),  // L para P3 → esquina de P1 (arriba-izq)
            new( 3f,  2.5f, 0),  // L para P4 → esquina de P2 (arriba-der)
        };

        // ── Paredes ───────────────────────────────────────────────────────
        MakeWall("Wall_Top",    new Vector3(0,  6.3f, 0), new Vector3(14, 0.5f, 1));
        MakeWall("Wall_Bottom", new Vector3(0, -6.3f, 0), new Vector3(14, 0.5f, 1));
        MakeWall("Wall_Left",   new Vector3(-7f, 0,   0), new Vector3(0.5f, 12, 1));
        MakeWall("Wall_Right",  new Vector3( 7f, 0,   0), new Vector3(0.5f, 12, 1));

        // Paredes internas que separan zonas (con huecos para pasar)
        MakeWall("Wall_HCenter_L", new Vector3(-3.5f, 0, 0), new Vector3(5.5f, 0.4f, 1));
        MakeWall("Wall_HCenter_R", new Vector3( 3.5f, 0, 0), new Vector3(5.5f, 0.4f, 1));
        // Hueco en el centro para cruzar (no se crea pared en x=0)

        // ── Obstáculos internos (dificultan cruzar en diagonal) ───────────
        MakeWall("Block_1", new Vector3(-1.5f,  2f, 0), new Vector3(0.7f, 1.5f, 1));
        MakeWall("Block_2", new Vector3( 1.5f, -2f, 0), new Vector3(0.7f, 1.5f, 1));

        // ── Suelo decorativo ───────────────────────────────────────────────
        var floor = MakeRect("Floor", Vector3.zero, new Vector3(13.5f, 12f, 1),
            new Color(0.1f, 0.1f, 0.15f));

        // ── GameManager ───────────────────────────────────────────────────
        var gmGO = new GameObject("PuzzleManager");
        var gm   = gmGO.AddComponent<PuzzleManager4>();
        Undo.RegisterCreatedObjectUndo(gmGO, "GM");

        // ── Palancas ──────────────────────────────────────────────────────
        var leverComponents = new Lever[4];
        for (int i = 0; i < 4; i++)
        {
            var lGO = MakeRect($"Lever_P{i+1}", leverPos[i],
                new Vector3(1.1f, 1.1f, 1), colors[i] * 0.4f);
            lGO.AddComponent<BoxCollider2D>().isTrigger = true;
            var lComp = lGO.AddComponent<Lever>();
            lComp.allowedPlayerIndex = i;
            lComp.leverColor = colors[i];

            // Etiqueta de quién puede activarla
            var label = new GameObject($"Label_L{i+1}");
            label.transform.SetParent(lGO.transform);
            label.transform.localPosition = new Vector3(0, 0.7f, 0);
            Undo.RegisterCreatedObjectUndo(label, "Label");

            leverComponents[i] = lComp;
        }

        // ── Key Spawn ─────────────────────────────────────────────────────
        var keySpawn = new GameObject("KeySpawn");
        keySpawn.transform.position = new Vector3(0, 0.3f, 0);
        Undo.RegisterCreatedObjectUndo(keySpawn, "KeySpawn");

        // ── Salida ────────────────────────────────────────────────────────
        var exitGO = MakeCircle("ExitZone", new Vector3(0, -5.5f, 0), 1f,
            new Color(0.5f, 0.5f, 0.5f, 0.6f));
        exitGO.AddComponent<ExitZone>();
        exitGO.SetActive(false);

        // ── Cámara ────────────────────────────────────────────────────────
        var cam = Camera.main != null ? Camera.main.gameObject : new GameObject("Main Camera");
        cam.transform.position = new Vector3(0, 0, -10);
        if (!cam.GetComponent<Camera>())
        {
            var c = cam.AddComponent<Camera>();
            c.orthographic = true;
            c.orthographicSize = 7f;
            c.backgroundColor = new Color(0.07f, 0.07f, 0.1f);
        }

        // ── Jugadores ─────────────────────────────────────────────────────
        for (int i = 0; i < 4; i++)
        {
            var pGO = MakeCircle($"Player_{i+1}", playerPos[i], 0.38f, colors[i]);
            pGO.tag = "Player";
            pGO.GetComponent<SpriteRenderer>().sortingOrder = 5;
            var rb = pGO.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0; rb.freezeRotation = true; rb.linearDamping = 10f;
            var pc = pGO.AddComponent<PuzzlePlayer4>();
            pc.playerIndex   = i;
            pc.controlScheme = controls[i];
            pc.playerColor   = colors[i];
        }

        // ── Asignar referencias al manager ────────────────────────────────
        gm.levers        = leverComponents;
        gm.keySpawnPoint = keySpawn.transform;
        gm.exitDoor      = exitGO;

        EditorUtility.SetDirty(gmGO);
        Debug.Log("[PuzzleGenerator] ✅ ¡Escena generada!");
        EditorUtility.DisplayDialog("¡Puzzle Generado!",
            "Escena creada. Pasos finales:\n\n" +
            "1. Crea un prefab con KeyItem.cs y tag 'Key'\n" +
            "   → asígnalo a PuzzleManager4 > keyPrefab\n\n" +
            "2. Añade un Canvas con TextMeshPro:\n" +
            "   · statusText\n" +
            "   · timerText\n" +
            "   · leversText\n" +
            "   · victoryPanel\n" +
            "   · penaltyPanel (Image roja)\n\n" +
            "3. Asigna la UI al PuzzleManager4\n\n" +
            "4. Añade el PortalTrigger al portal negro\n" +
            "   de tu escena del plataformero", "OK");
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    static GameObject MakeWall(string name, Vector3 pos, Vector3 scale)
    {
        var go = MakeRect(name, pos, scale, new Color(0.22f, 0.22f, 0.28f));
        go.AddComponent<BoxCollider2D>();
        return go;
    }

    static GameObject MakeRect(string name, Vector3 pos, Vector3 scale, Color color)
    {
        var go = new GameObject(name);
        go.transform.position   = pos;
        go.transform.localScale = scale;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        sr.color  = color;
        Undo.RegisterCreatedObjectUndo(go, name);
        return go;
    }

    static GameObject MakeCircle(string name, Vector3 pos, float radius, Color color)
    {
        var go = new GameObject(name);
        go.transform.position   = pos;
        go.transform.localScale = Vector3.one * radius * 2f;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        sr.color  = color;
        go.AddComponent<CircleCollider2D>();
        Undo.RegisterCreatedObjectUndo(go, name);
        return go;
    }
}
#endif
