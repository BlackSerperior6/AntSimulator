using ConsoleApp3;
using ScottPlot;

Random random = new Random(23);
/*List<int> results = new List<int>();

for (int i = 0; i < 101; i++)
    results.Add(random.Next(0, 2));

Console.WriteLine(string.Join(" ", results));*/

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

int levelCounter = 10;

Dictionary<string, double> yLevels = new Dictionary<string, double>();
Dictionary<AntState, List<(double time, double duration, AntState end)>> list = new Dictionary<AntState, List<(double time, 
    double duration, AntState end)>>();

foreach (var key in States.Keys)
    list[key] = new List<(double time, double duration, AntState end)>();

foreach (var key in States.Keys)
{
    yLevels.Add(key.ToString(), levelCounter);
    levelCounter++;
}

double currentTime = 0;

AntState currentState = AntState.StealBread;

Dictionary<double, List<double>> mediums = new Dictionary<double, List<double>>();

for (int i = 1; i <= 1000; i++)
{
    Dictionary<AntState, double> chances = new Dictionary<AntState, double>();

    foreach (var pair in States[currentState])
    {
        var roll = -1 / (double) pair.Value * Math.Log(random.NextDouble());
        chances.Add(pair.Key, roll);
    }

    var min = chances.OrderByDescending(x => x.Value).Last();

    list[currentState].Add(new (currentTime, min.Value, min.Key));

    currentState = min.Key;
    currentTime += min.Value;

    if (i % 10 == 0)
    {
        Dictionary<AntState, double> mediumTimes = new Dictionary<AntState, double>();

        foreach (var pair in list)
        {
            var timeSum = list.Sum(x => x.Value.Sum(v => v.duration));
            var medium = timeSum / currentTime;

            mediumTimes.Add(pair.Key, medium);
        }

        mediums[currentTime] = new List<double>();

        foreach (var pair in mediumTimes)
            mediums[currentTime].Add(pair.Value);
    }
}

var plt = new Plot();

foreach (var pair in list)
{
    foreach (var value in pair.Value)
    {
        var scatter = plt.Add.Scatter(value.time, yLevels[pair.Key.ToString()]);

        var progressArrow = plt.Add.Arrow(new Coordinates(value.time, yLevels[pair.Key.ToString()]),
                        new Coordinates(value.time + value.duration, yLevels[pair.Key.ToString()]));

        var loweringArrow = plt.Add.Arrow(new Coordinates(value.time + value.duration, yLevels[pair.Key.ToString()]),
                        new Coordinates(value.time + value.duration, yLevels[value.end.ToString()]));
    }
}