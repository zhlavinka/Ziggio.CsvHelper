using System.Collections;

namespace Ziggio.Csv;

public class PropertyAtlas : IEnumerable<PropertyMap> {
  private readonly Dictionary<string, PropertyMap> _dictionary = new();

  public int Count => _dictionary.Count;
  public bool IsInitialized { get; private set; } = false;

  public PropertyMap this[string name] {
    get => _dictionary[name];
  }

  public PropertyMap this[int index] {
    get => _dictionary.Values.First(map => map.HeaderIndex == index);
  }

  public PropertyAtlas() {
  
  }

  public void Add(PropertyMap map) {
    _dictionary.Add(map.HeaderName, map);
  }

  public bool Contains(PropertyMap map) {
    return _dictionary.ContainsKey(map.HeaderName);
  }

  #region IEnumerable
  public IEnumerator<PropertyMap> GetEnumerator() {
    return _dictionary.Values.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() {
    return _dictionary.GetEnumerator();
  }
  #endregion
}
