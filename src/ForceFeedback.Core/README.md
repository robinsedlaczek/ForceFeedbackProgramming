# Force Feedback Generation

Feedback is given to the programmer in the frontend. But which feedback to give is determined here in the core/backend.

However, the rules are not specified by the backend, but rather red from a config file. The `ForceFeedbackMachine`
just loads the rules and interprets them (see for example `NoiseGenerator`).

## Config files
The rules for the feedback are stated in config files with the name `.forcefeedbackprogramming`.
Config files can be located in difference places:

* where the solution resides the current method belongs to,
* where the project resides, or 
* where the file of the current method resides.

For a given source file the config file closest to it is consulted.

If no config file is found a config file with default values is created next to the solution.

## Feedback rules

The feedback config structure generally is as follows. Each rule consists of several properties:

* number of lines in function from when on a certain feedback should be applied

* Properties for **visual feedback**
  * the background color for the function in the IDE if the line number threshold has been reached; see the below
  image for to what color names relate to. See [here](http://www.99colors.net/dot-net-colors) for a list of color names.
  Alternatively colors can be given as RGB hex codes (e.g. `#8A2BE2` instead of `BlueViolet`).
  * the background color transparency; this is to make colors "lighter"/"darker" to get 
  the desired contrast for the text; smaller values mean less transparency, e.g. 0.1 (10%) is less transparent
  (more opaque, "darker") than 0.85 (85%).
  
![Accepted names for background colors](images/BackgroundColors.png)
  
* Properties for **tactile feedback**
  * after how many characters entered (or changes to source code) should noise be inserted into the source code?
  * how many noise characters should be inserted?
  * how many milliseconds should changes be delayed?

Config file example:

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
    {
      "MinimumNumberOfLinesInMethod": 51,

      "BackgroundColor": "Peru",
      "BackgroundColorTransparency": 0.0,

      "NoiseDistance": 20,
      "NoiseLevel": 5,

      "Delay": 0
    },
    {
      "MinimumNumberOfLinesInMethod": 101,

      "BackgroundColor": "Sienna",
      "BackgroundColorTransparency": 0.1,

      "NoiseDistance": 12,
      "NoiseLevel": 2,

      "Delay": 100
    }
  ]
}
```

Explanation of example:

* No feedback will be given for methods with less than 11 lines.
* Method with 11 up to 25 lines will be underlayed with a beige background with 0% transparency.
* Methods from 26 up to 50 lines with become burlywood colored (0% transparency) and changes will be hampered by 3
noise characters every 50 characters inserted.
* Methods with 51 to 100 lines will be peru colored (0%) and typing will be disturbed with 5 characters every 20 insertions.
* Finally from 101 lines on all methods will be colored in sienna (10% transparency) and every 12 insertions 2 noise chars
will be inserted - plus the typing will be delayed by 100msec.
