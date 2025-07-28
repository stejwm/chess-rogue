using System.Diagnostics;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

public class StockfishEngine : MonoBehaviour
{
    private Process stockfish;
    private StreamWriter stockfishInput;
    private StreamReader stockfishOutput;


    void Start()
    {
        StartEngine();
    }

    void OnApplicationQuit()
    {
        QuitEngine();
    }

    public void StartEngine()
    {
        string stockfishPath = Path.Combine(Application.streamingAssetsPath, "fairystockfish.exe");
        string stockFishVariantPath = Path.Combine(Application.streamingAssetsPath, "variants.ini");
        if (!File.Exists(stockfishPath))
        {
            UnityEngine.Debug.LogError("Stockfish executable not found at: " + stockfishPath);
            return;
        }

        stockfish = new Process();
        stockfish.StartInfo.FileName = stockfishPath;
        stockfish.StartInfo.UseShellExecute = false;
        stockfish.StartInfo.RedirectStandardInput = true;
        stockfish.StartInfo.RedirectStandardOutput = true;
        stockfish.StartInfo.CreateNoWindow = true;
        stockfish.Start();

        stockfishInput = stockfish.StandardInput;
        stockfishOutput = stockfish.StandardOutput;

        // Handshake
        SendCommand($"setoption name VariantPath value {stockFishVariantPath}");
        SendCommand("uci");
        ReadUntil("uciok");
        SendCommand("setoption name UCI_Variant value chess-rogue");
        SendCommand("setoption name MultiPV value 10");
        SendCommand("setoption name UCI_LimitStrength value true");
        SendCommand("setoption name UCI_Elo value 0");
        //SendCommand("setoption name Skill Level value -20");

        SendCommand("isready");
        ReadUntil("readyok");

        UnityEngine.Debug.Log("Fairy-Stockfish engine started.");
    }

    public async Task<string> GetBestMove(string fen, int depth = 8)
    {
        SendCommand($"position fen {fen}");
        SendCommand($"go depth {depth}");

        string line;
        while ((line = await stockfishOutput.ReadLineAsync()) != null)
        {
            if (line.StartsWith("bestmove"))
            {
                string[] parts = line.Split(' ');
                return parts.Length >= 2 ? parts[1] : null;
            }
        }

        return null;
    }
    public async Task<List<string>> GetTopMoves(string fen, int depth = 15)
    {
        // Set MultiPV option
        SendCommand($"position fen {fen}");
        SendCommand($"go depth {depth}");

        var moveList = new Dictionary<int, string>();
        string line;

        while ((line = await stockfishOutput.ReadLineAsync()) != null)
        {
            //UnityEngine.Debug.Log("[Stockfish] " + line);

            // Parse lines like: info depth 10 multipv 2 score cp 200 pv e2e4 e7e5 ...
            if (line.StartsWith("info") && line.Contains(" pv "))
            {
                var tokens = line.Split(' ');

                int multipvIndex = Array.IndexOf(tokens, "multipv");
                int pvIndex = Array.IndexOf(tokens, "pv");

                if (multipvIndex != -1 && pvIndex != -1 && pvIndex + 1 < tokens.Length)
                {
                    if (int.TryParse(tokens[multipvIndex + 1], out int pvNum))
                    {
                        string move = tokens[pvIndex + 1]; // First move in PV
                        moveList[pvNum] = move;
                    }
                }
            }

            if (line.StartsWith("bestmove"))
            {
                UnityEngine.Debug.Log($"[Stockfish] {line}");
                break; // We're done
            }
        }

        // Return moves ordered by PV rank
        return moveList.OrderBy(kv => kv.Key).Select(kv => kv.Value).ToList();
    }

    private void SendCommand(string command)
    {
        stockfishInput.WriteLine(command);
        stockfishInput.Flush();
    }

    private void ReadUntil(string keyword)
    {
        string line;
        while ((line = stockfishOutput.ReadLine()) != null)
        {
            if (line.Contains(keyword))
                break;
        }
    }
    private void ReadLines(int maxLines = 100)
{
    for (int i = 0; i < maxLines; i++)
    {
        string line = stockfishOutput.ReadLine();
        if (line == null)
            break;
        //UnityEngine.Debug.Log("[Stockfish] " + line);
    }
}

    public void QuitEngine()
    {
        if (stockfish != null && !stockfish.HasExited)
        {
            stockfishInput.WriteLine("quit");
            stockfishInput.Close();
            stockfishOutput.Close();
            stockfish.Kill();
            stockfish = null;
        }
    }
}