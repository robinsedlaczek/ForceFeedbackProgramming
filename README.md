# Force Feedback Programming

## Overview

Make evolvability really tangible while coding: that's what _Force Feedback Programming_ (FFP) is about.

There are tools which might be able to tell you during CI if you wrote clean code. Or your colleagues give you feedback on this during a review. But that's pretty delayed feedback. It's hard to learn from it for next time. To write clean code in the first place still is not inevitable.

That hopefully changes with FFP! Because FFP will tell you right away _while you're typing_ how you're doing on the evolvability front.

And not only will FFP tell you how clean your code is, it will actively hinder you to pollute it. _FFP gives you visual feedback plus tactile._ You'll experience a real force right at your fingertips! Writing less clean code will become tangibly harder.

And all this is made possible through a simple Visual Studio extension you can even [adapt to your coding style](CONFIG.md) and level of brownfield.

Enjoy!

PS: If you like what you see and have time to spare, you can join us in moving the FFP extension forward. Please [check the wiki](https://github.com/robinsedlaczek/ForceFeedbackProgramming/wiki) for details.

## Binaries

You can download the precompiled Visual Studio installer package from the [releases page](https://github.com/robinsedlaczek/ForceFeedbackProgramming/releases/ "Visual Studio Installer Package releases").

## Supported IDEs

### Microsoft Visual Studio

Force Feedback is currently available in Visual Studio only (exactly for VS 2015, 2017 and 2019). Therefore, we deliver a Visual Studio extension that can be found in the VS marketplace [here](https://marketplace.visualstudio.com/items?itemName=RobinSedlaczek.ForceFeedback) and that can be installed via the main menu entry "Tools\Manage Extensions". We update the extension in the marketplace with every stable version. Stable versions on the [releases page](https://github.com/robinsedlaczek/ForceFeedbackProgramming/releases/ "Visual Studio Installer Package releases") are those versions, that are free of any additional version status info (e.g. -alpha, -beta etc.).

### ABAB

The phrase "Visual Studio only" is not really correct. There are some guys who ported the Force Feedback Programming to SAP's [ABAB](https://en.wikipedia.org/wiki/ABAP). We got the hint via Twitter [here](https://twitter.com/ceedee666/status/1106887766221180929). You can find the GitHub repo for the ABAB implementation [here](https://github.com/css-ch/abap-code-feedback).

[!Please be aware that their implementation is no fork of our repo. The guys there are working independent from us currently. Their ABAB AiE integration is not part of our project and we are not responsible. In case of issues and/or questions, please contact them directly.]

### Roadmap

We have the integration for Visual Studio Code and JetBrains Rider on the list!

## Health of master (Release|x86): 

[![Build status](https://ci.appveyor.com/api/projects/status/mrnvhtnf9k2xrs4g/branch/master?svg=true)](https://ci.appveyor.com/project/robinsedlaczek/forcefeedbackprogramming/branch/master)

## Wanna chat with us?

You can meet us here: [![Gitter](https://badges.gitter.im/robinsedlaczek/ForceFeedbackProgramming.svg)](https://gitter.im/robinsedlaczek/ForceFeedbackProgramming?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
