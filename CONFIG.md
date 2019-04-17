# Configuring the VS Extension

The Visual Studio extension for Force Feedback Programming provides visual plus tactile feedback on the evolvability of your code while typing.

Since not all code is created equal and your team's coding style might differ from that of another team you can configure the extension's "sensitivity" and its "use of force".

Currently the extension uses the *number of lines* in a function/methode as its only metric to determine how clean/dirty your code is. We know this might sound simplistic, but we've thought long and hard about it and believe that more sophisticated metrics would only deliver pseudo accuracy.

The following method consists of 10 lines of code between the initial `{` and the final `}`. A background color is chosen accordingly:

![](images/config_fig1.png)

Now, adding a line (by breaking the last statement apart) pushes it into the next "brownfield category", though, and the background color changes:

![](images/config_fig2.png)

The visual feedback is immediate, as you see. So is the tactile feedback which consist of spurts of random characters inserted while you type and delaying the effect of your keystrokes.

The metric is simple, as you see, but how is the feedback chosen? That's what you can control with FFP config files:

## Locating the config file
The rules to assess methods and define the feedback to give are read from a config file with the name `.forcefeedbackprogramming`.

The FFP extension will use the first config file it finds while searching for it in a number of places in this order:

1. directory of current source file
2. directory of project the source file belongs to
3. directory of solution the project belongs to

If no config file is found, one is created from a default in the solution directory.

## Structure of config file
A config file is a JSON file with a simple structure. Here's an excerpt from the default config:

```
{
  "Version": "1.0",

  "FeedbackRules": [
    {
      "MinimumNumberOfLinesInMethod": 11,

      "BackgroundColor": "Beige",
      "BackgroundColorTransparency": 0.0,

      "NoiseDistance": 0,
      "NoiseLevel": 0,

      "Delay": 0
    },
    {
      "MinimumNumberOfLinesInMethod": 26,

      "BackgroundColor": "Burlywood",
      "BackgroundColorTransparency": 0.0,

      "NoiseDistance": 50,
      "NoiseLevel": 3,

      "Delay": 0
    },
    ...
  ]
}
```

Currently it's just a list of rules defining **levels of what's considered clean/dirty**.

* Levels relate to the number of lines in a method only (`MinimumNumberOfLinesInMethod`). The example states that less that 11 lines of code (LOC) is considered perfectly clean. No feedback is given. But from 11 LOC on you'll see/feel feedback. First a little - 11 to 25 LOC -, then more - 26 LOC and up. All methods with more LOC than the highest number in the rules state are covered by that rule.

**Visual feedback** is given for each level based on two properties:

* Visual feedback is given by coloring the backhground of a method's body (`BackgroundColor`). You can define the color by name, e.g. `Beige` or `Maroon` or any color from [this list](http://www.99colors.net/dot-net-colors). Or you can define it with a hex code, e.g. `#F0FFFF` instead of `Azure`; use a [tool like this](https://www.rapidtables.com/web/color/RGB_Color.html) to mix your color.
* Some colors might lead to bad contrast for the source text when applied in their default "strength" or "thickness". To adjust for better readability you can change the transparency with which they are applied (`BackgroundColorTransparency`). The default is `0.0`, i.e. no transparency/"full strength"/maximum opacity. Choose a value between `0.0` and `1.0` to make the background color more light, more translucent.

For **tactile feedback** you can tweak three properties:

* If keystrokes/changes should be delayed (to simulate wading through a brownfield), then set `Delay` to a millisecond value of your choice, e.g. `100` to delay every keystroke by 0.1 seconds.
* In addition you can add "noise" to the input. That means additional random characters will be inserted to hamper typing progress. `NoiseDistance` defines how many keystrokes/changes should pass without disturbance, e.g. `10` for "insert noise every 10 keystrokes".
* How many noise characters are inserted is defined by `NoiseLevel`, e.g. `3` to insert a random string like "♥☺❉".
