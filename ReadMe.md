  - **Status:** Stable and mature. Several projects are using this library,
    and it has received extensive testing on Linux and Windows.

  - **Platforms:** Cross-platform, developed on Linux but also tested and
    working without any known issues on Windows.

Nuclex.Support
==============

This library aims to be your trusty toolbox of supporting code for
problems that come up in any type of project. It consists of carefully
chosen and well-designed pieces that aid you in dealing with settings
storage, change notifications, stream processing, object cloning and more.

There are unit tests for the whole library, so everything is verifiably
working on all platforms tested (Linux, Windows, Raspberry).


Object Cloning
--------------

Whether you use the prototye design patten on complex objects or have another
reason, sometimes a deep clone of an object tree is needed. This library
provides three complete solutions to cloning objects in .NET:

- The `SerializationCloner`. It uses .NET's BinarySerializer in a way that
  will serialize your object tree regardless of whether your objects have
  the `Serializable` attribute or not. This is the slowest, least efficient
  object cloner, but it relies on built-in .NET classes only.

- The `ReflectionCloner` uses .NET's reflection capabilities (that means
  interrogating an object what fields and properties it has) to create
  complete clones of an object, including any arrays and referenced objects.
  This serializer has no setup time and has pretty decent performance.

- The `ExpressionTreeCloner` uses Linq expression trees to generate tailored
  cloning code for your classes at runtime. This method of cloning has a setup
  time (meaning it takes longer the first time it is confronted with a new
  class), but from the second clone onwards, is much faster than the others.

All three object cloners can create *shallow clones* (meaning any references
to other object will be kept without copying the referenced objects, too) and
*deep clones* meaning any refeferenced objects (and their referenced objects)
will be cloned as well. Careful, this means event subscribers, such a forms
and unexpected hangers-on will be cloned, too.

Furthermore, all three object cloners can create *property-based clones*
(where only those settings exposed via properties are cloned), which may skip
the non-exposed parts of an object, as well as *field-based clones* which
replicate all the data of a class - any private field and hidden state.

```csharp
class Example {
  public Example(Example child = null) {
  	Child = child;
  }
  public Example Child { get; private set; }
}

class Test {
  public static void CloneSomething() {
  	var test = new Example(new Example());

  	var reflectionCloner = new ReflectionCloner();
  	var clone = reflectionCloner.DeepFieldClone(test);

    // Clone is now a complete copy of test, including the child object
  }
}
```


Settings
--------

Many applications have to store their settings in an external file or,
for pure Windows applications, in the registry. This can be tedious and
difficult to unit test, too. Nuclex.Support provides an autonomous ini
parser (which works cross-platform and does **not** rely on
`GetPrivateProfileString`).

Furthermore, it uses an interface to provide the same functionality for
the Registry and in-memory settings. This lets you switch between storing
your settings in the registry, in an .ini file or constructing a settings
container in memory to appropriately unit-test your code with mock data.

```csharp
static readonly string BasicCategoryName = "Basic";
static readonly string HintsCategoryName = "Hints";

void saveSettings(ISettingStore settingsStore) {
  settingsStore.Set(BasicCategoryName, "AskSaveOnQuit", this.askSaveOnQuit);
  settingsStore.Set(BasicCategoryName, "ActivePanel", this.activePanelIndex);
  settingsStore.Set(HintsCategoryName, "ShowNameHint", this.showNameHint);
  // ...
}

void saveSettingsToIni() {
  var iniStore = new ConfigurationFileStore();
  saveSettings(iniStore);

  using(var writer = new StreamWriteR("awesome-app.ini")) {
  	iniStore.Save(writer);
  	writer.Flush()
  }
}

void saveSettingsToRegistry() {
  using(
  	var registryStore = new WindowsRegistryStore(
  	  RegistryHive.HKCU, "AwesomeApplication"
    )
  ) {
  	saveSettings(registryStore);
  }
}
```

Observable Base Class
---------------------

.NET provides the `INotifyPropertyChanged` interface for objects to expose
an event that reports when a property of the object has changed. This is
used by data binding UI controls and some ORMs to detect when an object has
been changed and the UI or database need to be updated.

It is a bit tedious to implement, so here's a base class to make it much
more pleasant to use:

```csharp
class CreateUserViewModel : Observable {

  public string FirstName {
  	get { return this.firstName; }
  	set {
  	  if(value != this.firstName) {
  	  	this.firstName = value;
  	  	OnPropertyChanged(nameof(FirstName));
  	  }
  	}
  }

  private string firstName;

}
```

There's an extension method for the consuming side, too, with proper handling
of *wildcard* change notifications that are often overlooked:

```csharp
CreateUserViewModel ViewModel { get; set; } 

void onPropertyChanged(object sender, PropertyChangedEventArgs arguments) {
  if(arguments.AreAffecting(nameof(ViewModel.FirstName))) {
  	this.firstNameLine.Text = ViewModel.FirstName;
  }
}
```
