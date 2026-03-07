#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Genera la escena "PuzzleScene2" — La Secuencia Oculta.
/// Menú: Tools > Puzzle > Generar Puzzle Secuencia
///
/// MAPA:
///   ╔══════════════════════════╗
///   ║   [P1🔵]     [P2🔴]    ║
///   ║                          ║
///   ║  [🔵]  [🔴]  [🟢]  [🟡]║  ← 4 placas en fila central
///   ║                          ║
///   ║   [P3🟢]     [P4🟡]    ║
///   ║                          ║
///   ║     ✦KEY    ⬡PORTAL     ║
///   ╚══════════════════════════╝
/// </summary>
public class PuzzleScene2Builder : MonoBehaviour
{
    [MenuItem("Tools/Puzzle/Generar Puzzle Secuencia")]
    static void Build()
    {
        Color[] colors = {
            new Color(0.25f, 0.55f, 1f),
            new Color(1f,    0.3f,  0.3f),
            new Color(0.25f, 0.9f,  0.35f),
            new Color(1f,    0.85f, 0.1f),
        };

        var controls = new PuzzlePlayer4.ControlScheme[]
        {
            PuzzlePlayer4.ControlScheme.WASD,
            PuzzlePlayer4.ControlScheme.Arrows,
            PuzzlePlayer4.ControlScheme.IJKL,
            PuzzlePlayer4.ControlScheme.Numpad,
        };

        // ── Cámara ─────────────────────────────────────────────────────────
        var camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 6.5f;
        cam.backgroundColor = new Color(0.05f, 0.03f, 0.1f);
        camGO.transform.position = new Vector3(0, 0, -10);
        Undo.RegisterCreatedObjectUndo(camGO, "Camera");

        // ── Paredes exteriores ──────────────────────────────────────────────
        MakeWall("Wall_Top",    new Vector3(0,  6f,  0), new Vector3(14f, 0.5f, 1));
        MakeWall("Wall_Bottom", new Vector3(0, -6f,  0), new Vector3(14f, 0.5f, 1));
        MakeWall("Wall_Left",   new Vector3(-7f, 0,  0), new Vector3(0.5f, 12f, 1));
        MakeWall("Wall_Right",  new Vector3( 7f, 0,  0), new Vector3(0.5f, 12f, 1));

        // ── Obstáculos internos ─────────────────────────────────────────────
        // Columnas que obligan a rodear para llegar a las placas
        MakeWall("Col_1", new Vector3(-3f,  2f, 0), new Vector3(0.6f, 2f,  1));
        MakeWall("Col_2", new Vector3( 3f,  2f, 0), new Vector3(0.6f, 2f,  1));
        MakeWall("Col_3", new Vector3(-3f, -2f, 0), new Vector3(0.6f, 2f,  1));
        MakeWall("Col_4", new Vector3( 3f, -2f, 0), new Vector3(0.6f, 2f,  1));

        // ── Suelo ───────────────────────────────────────────────────────────
        MakeRect("Floor", Vector3.zero, new Vector3(13.5f, 11.5f, 1),
            new Color(0.08f, 0.05f, 0.14f));

        // ── GameManager ─────────────────────────────────────────────────────
        var gmGO = new GameObject("SequencePuzzleManager");
        var gm   = gmGO.AddComponent<SequencePuzzleManager>();
        Undo.RegisterCreatedObjectUndo(gmGO, "GM");

        // ── Placas en fila central ──────────────────────────────────────────
        float[] plateX = { -4.5f, -1.5f, 1.5f, 4.5f };
        var plates = new SequencePlate[4];

        for (int i = 0; i < 4; i++)
        {
            var pGO = MakeRect($"Plate_{i}", new Vector3(plateX[i], 0f, 0),
                new Vector3(1.3f, 1.3f, 1), colors[i] * 0.35f);
            var col = pGO.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            var plate = pGO.AddComponent<SequencePlate>();
            plate.plateIndex = i;
            plate.plateColor = colors[i];
            plates[i] = plate;
        }

        // ── Key Spawn ────────────────────────────────────────────────────────
        var keySpawn = new GameObject("KeySpawn");
        keySpawn.transform.position = new Vector3(-1.5f, -4.5f, 0);
        Undo.RegisterCreatedObjectUndo(keySpawn, "KeySpawn");

        // ── Portal de salida ─────────────────────────────────────────────────
        var portalGO = MakeCircle("ExitPortal2", new Vector3(1.5f, -4.5f, 0),
            0.85f, new Color(0.4f, 0.1f, 1f, 0.8f));
        portalGO.AddComponent<ExitPortal2>();
        portalGO.SetActive(false);

        // ── Jugadores ────────────────────────────────────────────────────────
        Vector3[] spawnPos = {
            new(-5f,  4f, 0),
            new( 5f,  4f, 0),
            new(-5f, -4f, 0),
            new( 5f, -4f, 0),
        };

        for (int i = 0; i < 4; i++)
        {
            var pGO = MakeCircle($"Player_{i+1}", spawnPos[i], 0.38f, colors[i]);
            pGO.tag = "Player";
            pGO.GetComponent<SpriteRenderer>().sortingOrder = 5;
            var rb = pGO.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0; rb.freezeRotation = true; rb.linearDamping = 10f;
            var pc = pGO.AddComponent<PuzzlePlayer4>();
            pc.playerIndex   = i;
            pc.controlScheme = controls[i];
            pc.playerColor   = colors[i];
        }

        // ── Asignar referencias ───────────────────────────────────────────────
        gm.plates        = plates;
        gm.keySpawnPoint = keySpawn.transform;
        gm.exitPortal    = portalGO;

        EditorUtility.SetDirty(gmGO);
        Debug.Log("[PuzzleScene2Builder] ✅ Escena generada.");
        EditorUtility.DisplayDialog("¡Puzzle 2 Generado!",
            "Pasos finales:\n\n" +
            "1. Crea prefab Key con KeyItem.cs y tag 'Key'\n" +
            "   → asígnalo a SequencePuzzleManager > keyPrefab\n\n" +
            "2. Canvas con TMP:\n" +
            "   · statusText\n" +
            "   · sequenceText  (muestra ✓→?→?→?)\n" +
            "   · failCountText\n" +
            "   · victoryPanel\n" +
            "   · penaltyPanel\n\n" +
            "3. Añade 'PuzzleScene2' al Build Settings\n\n" +
            "4. Pon Portal2Trigger en tu portal del Nivel2_Parte2",
            "OK");
    }

    static void MakeWall(string n, Vector3 pos, Vector3 scale)
    {
        var go = MakeRect(n, pos, scale, new Color(0.2f, 0.15f, 0.3f));
        go.AddComponent<BoxCollider2D>();
    }

    static GameObject MakeRect(string n, Vector3 pos, Vector3 scale, Color color)
    {
        var go = new GameObject(n);
        go.transform.position   = pos;
        go.transform.localScale = scale;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        sr.color  = color;
        Undo.RegisterCreatedObjectUndo(go, n);
        return go;
    }

    static GameObject MakeCircle(string n, Vector3 pos, float radius, Color color)
    {
        var go = new GameObject(n);
        go.transform.position   = pos;
        go.transform.localScale = Vector3.one * radius * 2f;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        sr.color  = color;
        go.AddComponent<CircleCollider2D>();
        Undo.RegisterCreatedObjectUndo(go, n);
        return go;
    }
}
#endif
