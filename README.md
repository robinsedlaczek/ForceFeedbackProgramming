# Force Feedback Programming

## Overview

Make evolvability really tangible while coding: that's what _Force Feedback Programming_ (FFP) is about.

There are tools which might be able to tell you during CI if you wrote clean code. Or your colleagues give you feedback on this during a review. But that's pretty delayed feedback. It's hard to learn from it for next time. To write clean code in the first place still is not inevitable.

That hopefully changes with FFP! Because FFP will tell you right away _while you're typing_ how you're doing on the evolvability front.

And not only will FFP tell you how clean your code is, it will actively hinder you to pollute it. _FFP gives you visual feedback plus tactile._ You'll experience a real force right at your fingertips! Writing less clean code will become tangibly harder.

And all this is made possible through a simple Visual Studio extension you can even [adapt to your coding style](CONFIG.md) and level of brownfield.

Take as an example this screenshot from [a video by contributor Robin](https://vimeo.com/171889390) where he's explaining how FFP works:

![](images/example.png)

As you see, methods in the source code are colored differently according to how clean/dirty FFP deems them. But this color is not static! It will change while you're editing your code because with each keystroke it might become more or less clean.

That's not all, however. In addition to this kind of visual feedback the FFP extension gives tactile feedback. [Read Robin's article](https://robinsedlaczek.com/2016/06/23/introducing-force-feedback-programming/) and watch his video to see that in action.

Enjoy!

PS: If you like what you see and have time to spare, you can join us in moving the FFP extension forward. Please [check the wiki](https://github.com/robinsedlaczek/ForceFeedbackProgramming/wiki) for details.

## Binaries

You can download the precompiled Visual Studio installer package from the [releases page](https://github.com/robinsedlaczek/ForceFeedbackProgramming/releases/ "Visual Studio Installer Package releases").

## Supported IDEs

### Microsoft Visual Studio

Force Feedback Programming is currently available for C# in Visual Studio only (VS 2015, 2017 and 2019). It's delivered as a Visual Studio extension that can be found in the VS marketplace [here](https://marketplace.visualstudio.com/items?itemName=RobinSedlaczek.ForceFeedback) and that can be installed via the main menu entry "Tools|Manage Extensions". We update the extension in the marketplace with every stable version. Stable versions on the [releases page](https://github.com/robinsedlaczek/ForceFeedbackProgramming/releases/ "Visual Studio Installer Package releases") are those versions, which are free of any additional version status info (e.g. -alpha, -beta etc.).

### ABAB

To be honest, the phrase "Visual Studio only" might not really be correct anymore. It seems there are some guys who have taken the Force Feedback Programming idea to SAP's [ABAB](https://en.wikipedia.org/wiki/ABAP). [Here's a Tweet](https://twitter.com/ceedee666/status/1106887766221180929) hinting at that. A repo for their ABAB implementation [ABAB implementation](https://github.com/css-ch/abap-code-feedback) is on GitHub.

*Please note: Their implementation is no fork of our repo. It's an independent project. Their ABAB AiE integration is not part of our effort here.*

### Roadmap

We have the integration for Visual Studio Code and JetBrains Rider on our list!

## Health of master (Release|x86): 

[![Build status](https://ci.appveyor.com/api/projects/status/mrnvhtnf9k2xrs4g/branch/master?svg=true)](https://ci.appveyor.com/project/robinsedlaczek/forcefeedbackprogramming/branch/master)

## Wanna chat with us?

You can meet us here: [![Gitter](https://badges.gitter.im/robinsedlaczek/ForceFeedbackProgramming.svg)](https://gitter.im/robinsedlaczek/ForceFeedbackProgramming?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
