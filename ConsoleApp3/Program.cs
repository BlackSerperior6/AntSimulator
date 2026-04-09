using ConsoleApp3;
using ScottPlot;

Random random = new Random(23);

Dictionary<AntState, List<(double time, double duration, AntState end)>> simulation(double maxTime, Dictionary<AntState, Dictionary<AntState, double>> states, 
    out double accumulatedTime)
{
    accumulatedTime = 0;

    Dictionary<AntState, List<(double time, double duration, AntState end)>> results = 
    new Dictionary<AntState, List<(double time, double duration, AntState end)>>();

    AntState currentState = AntState.StealBread;

    foreach (var key in states.Keys)
        results[key] = new List<(double time, double duration, AntState end)>();

    while (accumulatedTime <= maxTime)
    {
        Dictionary<AntState, double> chances = new Dictionary<AntState, double>();

        foreach (var pair in states[currentState])
        {
            var roll = -1 / (double) pair.Value * Math.Log(random.NextDouble());
            chances.Add(pair.Key, roll);
        }

        var min = chances.OrderByDescending(x => x.Value).Last();

        results[currentState].Add(new (accumulatedTime, min.Value, min.Key));

        currentState = min.Key;
        accumulatedTime += min.Value; 
    }

    return results;
}

Dictionary<AntState, Dictionary<AntState, double>> States = new Dictionary<AntState, Dictionary<AntState, double>>() 
{
    {AntState.StealBread, new Dictionary<AntState, double>
    {
        {AntState.ReturnToColony, 0.7d },
        {AntState.StealCheese, 0.8d },
        {AntState.ScareDad, 0.1d }
    } },
    {AntState.ReturnToColony, new Dictionary<AntState, double> 
    {
        {AntState.ScareDad, 0.8d },
        {AntState.StealCheese, 0.5d },
        {AntState.StealBread, 0.1d }
    } },
    {AntState.ScareDad, new Dictionary<AntState, double>
    {
        {AntState.ReturnToColony, 0.6d },
        {AntState.StealCheese, 0.9d },
        {AntState.StealBread, 0.6d }
    } },
    {AntState.StealCheese, new Dictionary<AntState, double>
    {
        {AntState.ReturnToColony, 0.7d },
        {AntState.ScareDad, 0.8d },
        {AntState.StealBread, 0.6d }
    } },
};

double levelCounter = 0.1;
Dictionary<string, double> yLevels = new Dictionary<string, double>();

foreach (var key in States.Keys)
{
    yLevels.Add(key.ToString(), levelCounter);
    levelCounter++;
}

var modelOneDay = simulation(24, States, out var accumlatedTime);

/*using (var plt = new Plot())
{
    double[] x = times.ToArray();
    int colorsCounter = -1;
    
    foreach(var pair in mediumTimes)
    {
        colorsCounter++;
    
        if (colorsCounter >= Colors.Category10.Length)
            colorsCounter = 0;
        
        var color = Colors.Category10[colorsCounter];

        double[] y = pair.Value.ToArray();

        var scatter = plt.Add.Scatter(x, y);
        scatter.Color = color;
        scatter.LineWidth = 2;
    }

    plt.SaveSvg("AntsAvaragePlot.svg", 2000, 4000);
}*/

using (var plt = new Plot())
{
    foreach (var pair in modelOneDay)
    {

        foreach (var value in pair.Value)
        {
            var scatter = plt.Add.Scatter(value.time, yLevels[pair.Key.ToString()]);

            var progressArrow = plt.Add.Arrow(new Coordinates(value.time, yLevels[pair.Key.ToString()]),
                            new Coordinates(value.time + value.duration, yLevels[pair.Key.ToString()]));
            
            progressArrow.ArrowFillColor = Colors.Black;
            progressArrow.ArrowLineColor = Colors.Black;

            var loweringArrow = plt.Add.Arrow(new Coordinates(value.time + value.duration, yLevels[pair.Key.ToString()]),
                            new Coordinates(value.time + value.duration, yLevels[value.end.ToString()]));
            
            loweringArrow.ArrowFillColor = Colors.Black;
            loweringArrow.ArrowLineColor = Colors.Black;
        }
    }

    plt.Axes.Left.SetTicks(yLevels.Values.ToArray(), yLevels.Keys.ToArray());
    plt.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericFixedInterval(0.1);
    plt.Axes.SetLimitsX(0, 24);
    plt.SaveSvg("AntsMovementsPlot.svg", 16000, 500);
}









