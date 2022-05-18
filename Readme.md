# UnityUtilityAiSystem
Ui tool for Createing Utility Ai Systems for Unity -- Not production Ready
UAI Creator – Guide

Welcome
This is the official guide for the Unity tool UAI Creator.
Make sure to use the newest version of the guide, which can be found at: https://github.com/DevOelgaard/UnityUtilityAiSystem/tree/master

-- If you are one of those types, who likes a visual description you can use: [UAI Creator - Guide.pdf](https://github.com/DevOelgaard/UnityUtilityAiSystem/files/8715986/UAI.Creator.-.Guide.pdf)

--- Yes I know it should all be avaiable here, and it will be in time :)


This tool is currently a prototype and by no means ready for production. You are free to use it as you see fit but be aware that it isn’t a priority to make the updates backwards compatible at this point.
Feedback is much welcomed, you can reach me at: DevOelgaard@gmail.com.
Thank you for trying the UAI Creator.
 
1	Quick Start
This chapter helps you get started with a simple ‘hello world’ example.
This guide was made for Unity version 2021.2.10f1 on Windows, if you are running a different version or system, the guide might not be 100% accurate.
Unity version:
Make sure your Unity version is >2021.2.10f1.
Follow this Link to get the newest Unity version: https://unity3d.com/get-unity/update.
1.1	Installation
1.	Open Package Manager: Window -> Package Manager
 
2.	In top-left corner of package manager: + -> Add package from git URL…
 
3.	Add one of the following links and press ‘Add’
a.	HTTPS: https://github.com/DevOelgaard/UnityUtilityAiSystem.git
b.	SSH: git@github.com:DevOelgaard/UnityUtilityAiSystem.git (Requires SSH Key)
c.	For help See: https://docs.unity3d.com/Manual/upm-git.html#Git-HTTPS 
 
4.	The package is now installed and ready for use.
 
 
1.2	Hello World
1.2.1	Code
1.	Create a new C# script anywhere in your assets folder and name it “PrintTxt.cs” and open it in your favorite IDE.
2.	Place the following code inside the PrintTxt.cs file
using System.Collections.Generic;
using UnityEngine;

public class PrintTxt : AgentAction
{

    // Here you define the parameters to be shown in the various UIS
    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>
        {
            // The format is: new Parameter(ParameterName, DefaultValue)
            // The Parameter supports all primitive types and some others like Unity.Color
            new Parameter("Text to print", "")
        };
    }

    // Called the first time the action is selected
    public override void OnStart(IAiContext context)
    {
        PrintText(context);
    }

    // Called at each successive selection of the action
    public override void OnGoing(IAiContext context)
    {
        PrintText(context);
    }

    private void PrintText(IAiContext context)
    {
        var text = GetParameter("Text to print").Value as string;

        var agent = context.Agent as AgentMono;
        
        Debug.Log(agent.name + ": " + text);
    }
}
1.2.2	UAI Template
1.	Open UAI Creator -> Template Manager
 
2.	If prompted press “Create New Project” and select an empty folder named “HelloWorld” anywhere on your system.
a.	WARNING!!! If you chose a non-empty folder you might lose the content of the folder
3.	In the ‘AIs’ category Add element -> Uai
 
4.	Double click the new Uai-Template (Uai)
 
5.	Name the UAI: ‘Hello World UAI’
 
6.	Add bucket
 
7.	Expand the bucket and add Consideration ‘Fixed Value’
 
8.	In Decisions tab: Add decision
 
9.	Expand the decision and add Consideration ‘Fixed Value’
 
10.	In actions tab add the Action: ‘Print Txt’. This is the action you implemented in code before
 
11.	Expand the Print Txt action and input ‘Hello World’ in the ‘Text to print’ field. You defined this by overriding the GetParameters() method in the code.
 
12.	In the toolbar: File -> Save and wait for the top left name to say: “HelloWorld – Ready”.
a.	You can now reload you project by: File -> Reload Project
 

13.	In the top-right corner Toggle ‘Playable’ On
 

14.	Close the Template Manager
1.2.3	Add an agent
1.	In the Hierarchy: Right Click -> 3D Object -> Cube and name it ‘Agent’
 
2.	In select the ‘Agent’ and in its inspector: Add Component -> Agent Mono
 
3.	In the Agent Mono component, set the default Ai to ‘Hello World UAI’
 
a.	If the AI name isn’t shown return to the Template Manager and press ‘Force update Playable Ais’ – This error can happen right after installing the package
 

1.2.4	Finally
1.	Run the game
2.	In the toolbar: UAI Creator -> Settings. To open the UAI Settings
 
3.	Set the mode to ‘Unrestricted’ and toggle ‘Run’ on
 
4.	 The agent prints Hello World To the console every frame
 
1.2.5	Challenge
Make the agent:
1.	Say: ‘Hi’ the first time the action is selected
2.	Count the number of consecutive selections and print it to console
3.	Change color on every selection Hint inn the Template Manager press the ‘Include Demo Files’, to get access to more considerations and actions
1.2.6	Challenge – Solution
1: Change the OnStart method in the PrintTxt method
2: Use the OnGoing method and a private property in the PrintTxt method
3: Add the change color action to the Hello World UAI







