# Help Your Users Help You to Reproduce Bugs

![Reproduce Bugs](https://u2ublogimages.blob.core.windows.net/peter/ReproduceBugs.jpg)
> Photo by rawpixel on Unsplash

I think we have all been there where a user reports a bug and you have trouble reproducing the bug on your system (Hey, it works on my machine). Windows comes with a built-in tool that will allow users to show you how they get the bug: the Problem Steps Recorder. The Problem Steps Recorder will record the user's interaction so you can easily reproduce the steps to get to the problem.

## Reproducing Bugs

As a developer you know that to fix a bug you have to reproduce it first. Many times users enter data using their particular workflow that results in a bug, but because you don't know about their way of working you can't reproduce it. So how can you learn about this workflow. One way is to sit next to the user and ask them to use the software until they get to the problem. But guess what. It doesn't happen when you're around...

## Introducing the Steps Recorder

Your windows operating system has a built in tool that allows end-users to record their interaction with the system in a very simple way. This tool is the **Steps Recorder**. To find it search your system using 'Steps Recorder'

![Search the Steps Recorder](https://u2ublogimages.blob.core.windows.net/peter/FindingStepsRecorder.PNG)

Or click the Start icon ![Start icon](https://u2ublogimages.blob.core.windows.net/peter/Start.PNG), scroll down to **Windows Accessories**, and look for the **Steps Recorder**.

## Recording the User's Interaction

Now that the problem steps recorder is running, a user can simply click on the **Start Record** button, interact with the system until they reach the cause of the bug. Along the way the user can pauze the recording and add comments.

![Steps Recorder](https://u2ublogimages.blob.core.windows.net/peter/StepsRecorder.PNG)

When the user is done they click the **Stop Record** button. The user can review the recording ensuring no sensitive stuff has been recorded, and then they can save the recording as a .zip file and email it to you.

## Reviewing the Problem Steps

Unzip the problem steps recording file and open the .mht file with Internet Explorer. Every click made by the user results in a screenshot which you can now easily review...


