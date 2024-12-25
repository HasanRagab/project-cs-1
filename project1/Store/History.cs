
namespace project1.Store;

public class History
{
    private readonly List<string> _history;
    private int _currentIndex;
    public History()
    {
        _history = new List<string>();
        _currentIndex = -1;
    }
    public int Count => _history.Count;
    public void AddInput(string input)
    {
        if (!string.IsNullOrWhiteSpace(input))
        {
            _history.Add(input);
            _currentIndex = _history.Count;
        }
    }
    public void DisplayHistory()
    {
        foreach (var item in _history)
        {
            Console.WriteLine(item);
        }
    }
}
