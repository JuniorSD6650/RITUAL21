using Ritual21.Models;
using Ritual21.SpecialCards;

namespace Ritual21.Core;

public class Mazo
{
    private List<Carta> _cartas = new List<Carta>();
    private Random _random = new Random();

    public Mazo()
    {
        GenerarMazo();
    }

    private void GenerarMazo()
    {
        // 1. Generar 30 Sbirros (Comunes) - Valores del 2 al 9
        for (int i = 0; i < 30; i++)
        {
            int valor = _random.Next(2, 10);
            _cartas.Add(new Carta($"Demorfito Sbirro #{i + 1}", valor));
        }

        // 2. Generar 11 Guardianes (Élite) - Valor 10
        for (int i = 0; i < 11; i++)
        {
            _cartas.Add(new Carta("Guardian de Élite", 10));
        }
        // 3. Generar 5 Espectros Legendarios y 5 Chamanes Ancestrales
        for (int i = 0; i < 5; i++) _cartas.Add(new Espectro());
        for (int i = 0; i < 5; i++) _cartas.Add(new Chaman());

        // 4. Generar 1 Archidemorfo (Jefe Supremo) - Valor 0 (se calcula al invocar)
        _cartas.Add(new Beholder());

        Console.WriteLine($"[MAZO]: Ritual sellado. El Archidemorfo ha sido incluido.");
    }

    // Algoritmo profesional para barajar (Fisher-Yates)
    public void Barajar()
    {
        int n = _cartas.Count;
        while (n > 1)
        {
            n--;
            int k = _random.Next(n + 1);
            Carta valor = _cartas[k];
            _cartas[k] = _cartas[n];
            _cartas[n] = valor;
        }
        Console.WriteLine("🔮 El mazo ha sido barajado mediante el ritual.");
    }

    public Carta RobarCarta()
    {
        if (_cartas.Count == 0) throw new Exception("El mazo está vacío. El ritual ha terminado.");

        Carta carta = _cartas[0];
        _cartas.RemoveAt(0);
        return carta;
    }
}