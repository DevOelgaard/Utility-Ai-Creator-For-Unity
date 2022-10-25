# Utility Ai Creator - _Fast implementation of Utility Ai in Unity_

[![](https://img.shields.io/static/v1?label=Sponsor&message=%E2%9D%A4&logo=GitHub&color=%23fe8e86)](https://github.com/sponsors/DevOelgaard)<br>

Becoming a sponsor will help this project stay alive, and make me a happy man :).

## What is Utility Ai Creator (UAC)?

UAC consists of three parts
- A collection of highly performant Utility Ai algorithms
- A framework for building complex Utility Ais, with a minimum amount of coding
- A UI editor tool for Unity, which allows developers to effortlessly develop and maintain Utility Ais

## Why UAC?

Creating any Ai for a game will almost certainly be a cumbersome task. It requires intricate knowledge of the chosen Ai architecture and a modern Ai is among other things required to be:
- Performant
- Easily maintainable
- Modifiable
- Dynamic
- Customizable

The UAC comes out of the box with a set of highly performant decision making algorithms, and provides settings for ensuring a minimum FPS or restricting the Ai to a certain time budget. <br>
The UAC allows a large amount of customization, through the  use of inheritance.

## Requirements

- Unity Version 2021.2.10f1 or higher. [Get newest version](https://unity3d.com/get-unity/update)

## Documentation and Diagrams

Documentation in the form of Rich Pictures, UML, C4 and more can be found under [Documentation](Diagrams)<br>

## Example Code

Sample code can be found in the [UAC-Show Case](https://github.com/DevOelgaard/UAC-Showcase) repository<br>

## Support

For support join: [Support Chanel Discord](https://discord.gg/3Pa2mmDE9a)

## Quick Start

This chapter helps you get started with a simple ‘hello world’ example.
This guide was made with UAC V. 1.11.1 and Unity v. 2021.2.10f1 running on Windows 10, if you are running a different version or OS, the guide might not be 100% accurate.

### Installation

1. Open Package Manager: Window -> Package Manager
2. In top-left corner of package manager: + -> Add package from git URL
3. Add one of the following links and press 'Add'. _[Get help](https://docs.unity3d.com/Manual/upm-git.html#Git-HTTPS)_
   - Https: https://github.com/DevOelgaard/UnityUtilityAiSystem.git
   - SSH: git@github.com:DevOelgaard/UnityUtilityAiSystem.git (Requires SSH Key)
4. The package is now installed and appears in the Package manager
   - If you don't see it make sure to select the filter: "Packages: In Project", in the package manager toolbar

### Hello World

#### Scripting

1. Create 2 C# scripts anywhere in your assets/scripts folder and name them "PrintTxt.cs" and "GameHandler.cs"
2. Place the following code inside the "PrintTxt.cs" file
```csharp
using UnityEngine;

public class PrintTxt: AgentAction
{
   public PrintTxt()
   {
       // Add parameters in the constructor
       AddParameter("Text to print","");
   }

    public override void OnStart(IAiContext context)
    {
        // Read the value of the parameter defined above. The parameterName is case sensitive
        var textFromParameter = ParameterContainer.GetParamString("Text to print").Value;
     
        // Get the agent from the Context and cast him as AgentMono
        // Don't think to much about the casting to AgentMono, just do it ;)
        var agent = context.Agent as AgentMono;
     
        // Printing to the console
        Debug.Log(agent.name +": " + textFromParameter);
    }
}
```
3. Place the following code inside the "GameHandler.cs"
```csharp
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    private AgentMono agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GameObject.Find("Agent").GetComponent<AgentMono>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.ActivateNextAction();
    }
}

```
<br>

#### Template Manager - Setting up the Ai

1. In the top toolbar of Unity: UAI Creator -> Template Manager<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/Open%20Template%20Manager.png "Open Template Manager")
   1. If prompted press "Create New Project"
   2. Create an empty folder, name it "HelloWorld" and select it
2. In the 'AIs' category: Add element -> Uai<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/Add%20first%20UAI.png "Add first UAI")
3. Double click the new Uai, which just appeared<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/Double%20Click%20UAI.png "Double Click UAI")
4. Name the UAI: 'Hello World'<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/Rename%20UAI.png "Rename UAI")
5. Add a bucket<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/Add%20a%20bucket.png "Add a bucket")
6. Expand the bucket by pressing _Expand_ (On the left side of the newly added bucket)
7. Add Consideration "Fixed Value"<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/Add%20Consideration%20Fixed%20Value.png "Add Consideration Fixed Value")
8. Select the _Decisions_ Tab
9. Add a decision<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/Add%20decision.png "Add decision")
10. Expand the decision by pressing _Expand_ (On the left side of the newly added decision)
11. Add Consideration "Fixed Value"<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/Add%20consideration%20to%20decision.png "Add consideration to decision")
12. Select the _Actions_ Tab
13. Add the Action "Print Txt". _This is the action you implemented earlier_<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/Add%20PrintTxt.png "Add PrintTxt")
14. Expand the "Print Txt" Action by pressing _Expand_
15. In the _Text to print_ field input: "Hello World". _You defined this in code in the **AddParameter** method_<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/Input%20Hello%20World.png "Input Hello World")
16. In the template managers toolbar: File -> Save
    - _If you lose your work, you can now reload your project by: File -> Reload Project_<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/FileSave.png "Save File")
17. In the top-right corner Toggle **Playable** on
    - _This tells the UAC, that the Ai can be used in the game_<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/PlayAbleOn.png "Toggle Playable On")
18. Close the Template manager

#### Unity - Creating the agent

1. Navigate to your scene view
2. In the _Hierarchy_: Right Click -> 3D Object -> Cube
3. Name it: "Agent"<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/AddAgent.png "Add Agent")
4. Select the Agent
5. In the Agents _Inspector_: Add Component -> Agent Mono<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/Add%20AgentMono.png "Add AgentMono")
6. In the _Agent Mono Component_: Set the default Ai to **Hello World**. 
   - _This is the AI you created in the template manager_<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/Select%20Hello%20World%20Uai.png "Select Hello World")
   - If the Ai isn't shown in the _default Ai_ drop down. Return to the template manager and press **Force Update Playable Ais**. _this error can happen right after installing the package_
7. In the _Hierarchy_: Right Click -> Create Empty
8. Name it: "GameHandler"
9. In the GameHandlers _Inspector_: Add Component -> GameHandler

#### Run the game - With your new Utility Ai

1. In Unity's main window: Pres **Play** to run the game
2. Watch the Console to see the Agent printing Hello World

### Challenge

Make the agent:
1. Say: "Hi" the first time the action is selected
2. Count the number of consecutive selections and print it to the console

### Challenge - Solution

1. Change the PrintTxt.Cs file to look like this (Or ad a new file/class named something else):
```csharp
using UnityEngine;

public class PrintTxt: AgentAction
{
   public PrintTxt()
   {
       // Add parameters in the constructor
       AddParameter("Text to print","");
   }

   // The OnStart method is called on the first selection
    public override void OnStart(IAiContext context)
    {
        var agent = context.Agent as AgentMono;
     
        // Printing to the console
        Debug.Log(agent.name +": Hi");
    }

    // The OnGoing method is called on every consecutive selection
    public override void OnGoing(IAiContext context)
    {
        // Read the value of the parameter defined above. The parameterName is case sensitive
        var textFromParameter = ParameterContainer.GetParamString("Text to print").Value;
     
        // Get the agent from the Context and cast him as AgentMono
        // Don't think to much about the casting to AgentMono, just do it ;)
        var agent = context.Agent as AgentMono;
     
        // Printing to the console
        Debug.Log(agent.name +": " + textFromParameter);
    }

    // The OnEnd method is called, when the decision was selected last evaluation, but not this evaluation
    public override void OnEnd(IAiContext context)
    {
        var agent = context.Agent as AgentMono;
     
        // Printing to the console
        Debug.Log(agent.name +": Bye");
    }
}
```
2. Change the PrintTxt.Cs file to look like this (Or ad a new file/class named something else):
```csharp
using UnityEngine;

public class PrintTxt: AgentAction
{
    private int consecutiveSelectionsCounter = 0;

   public PrintTxt()
   {
       // Add parameters in the constructor
       AddParameter("Text to print","");
   }

    // The OnGoing method is called on every consecutive selection
    public override void OnGoing(IAiContext context)
    {
        // Incrementing the counter.
        consecutiveSelectionsCounter++;
        
        // Printing to the console
        Debug.Log("Consecutive Selections: " + consecutiveSelectionsCounter);
    }

    // The OnEnd method is called, when the decision was selected last evaluation, but not this evaluation
    public override void OnEnd(IAiContext context)
    {
        // Resetting counter when, the action isn't selected
        consecutiveSelectionsCounter = 0;
    }
}
```

## Overview

This chapter is meant to give you an overview of the UAI. You can read it all or skip to the sections representing the topics you wish to learn more about. After reading this chapter you will have a general understanding of, what was happening during the Hello World example.

### The Basics

A UAI is created by putting together multiple parts. At the top level you have the **UAI**, which consists of a collection of **Buckets**, each **bucket** holds a collection of **Considerations** and a collection of **Decisions**. Each **decision** holds a collection of **Considerations** and **AgentActions**.<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/UAI%20Rich%20Picture.png "UAI Rich Picture")

**UAI**: Used by an agent as a container for all UAI objects, including decision making objects (more on that later).<br>
**Bucket**: Collection for a group of similar **Decisions**, which shares one or more **Considerations**.<br>
**Decision**: Holds **AgentActions** and **Considerations** relevant to those actions. <br>
**Consideration**: Reads data from the game world and uses a response curve to convert it to a score between 0-1, where a higher score is interpreted as a higher usefulness for the agent.<br>
**AgentAction**: Executes code in the attached system / game.<br>
**ResponseCurve**: Used to model how data extracted with a consideration should be converted to a score.<br> 

#### UAI

The UAI consists of a collection of Buckets and 4 Decision Making Objects. By attaching a UAI to an agent, the agent will be able to select actions from within the UAIs **buckets**.<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/UAI%20Components.png "UAI Components")

#### Bucket

A bucket has a **utility**, a **weight**, a collection of **Considerations** and a collection of **Decisions**. The utility is calculated by the UAIs **Utility Scorer** and multiplied by the buckets **weight**. The **decisions** individual utilities are also multiplied by the buckets **weight**.<br>
A bucket should be used to group all decisions which shares _(or could share)_ the same considerations, for example all ranged attacks, could be placed in the same bucket called _Ranged Attacks_ and a common consideration would be: “Do I have a ranged weapon”.<br>
If any of the buckets considerations returns a score of 0, it indicates that all decisions in the bucket are invalid. I.e., if the agents don’t have a ranged weapon, he shouldn’t consider any ranged decisions.
The weight defines the priority of the bucket compared to other buckets, where a higher weight equals a higher priority, by increasing the likelihood of the buckets decisions being selected, (due to a higher total utility).<br>
The utility represents how much the agent would benefit from choosing this decision. The rule for calculating the utility is defined by the **Utility Scorer**.<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/Bucket%20RichPicture.png "Bucket Rich Picture")

#### Decision

The **Decision** has a **utility**, a collection of **Considerations** and a collection of **AgentActions**. The score is calculated using the parent UAIs **Utility Scorer** and is multiplied by the parent Buckets weight.<br>

A **Decision** represents a specific set of actions an agent can take, like “Fire bow at target”. <br>
The **Considerations** should be relevant to the given decision. I.e., for the decision “Fire bow at target”, the considerations could be: “Chance to hit target”, “Potential damage to target” and “Desire to damage target”.
The utility represents how much the agent would benefit from choosing this decision. The rule for calculating the utility is defined by the **Utility Scorer**.
The actions define the code, that must be executed in the game. Int the “Fire bow at target” example, the actions could be: “Animate fire bow”, “Deal damage to target” and “Reduce ammunition”. Another approach could be to use a single action called: “Fire bow at target”, and let all logic concerning animation, damage, and ammunition be handled by the game, but this depends on the given use case.<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/Decision%20RichPicture.png "Decision Rich Picture")

##### Finding best target

```csharp
using UnityEngine;

public class DecisionAllTargets:Decision
{
    protected override float CalculateUtility(IAiContext context)
    {
        var bestUtility = float.MinValue;
        GameObject bestTarget = null;
        // Finding all targets with basic Unity method
        var targets = GameObject.FindGameObjectsWithTag("Target");

        foreach (var target in targets)
        {
            // Storing the target in the AI context and makes it available for all
            // Child Considerations and AgentActions of the decision
            context.SetContext("Target",target,this);

            // Getting utility for current target
            var utility = Evaluate(context);

            if (utility > bestUtility)
            {
                bestTarget = target;
                bestUtility = utility;
            }
        }
        
        // Storing the best target to be used by AgentActions if the decision is selected
        context.SetContext("Target",bestTarget,this);
        
        // returning the best utility
        return bestUtility;
    }
}
```


#### Consideration

The consideration can be considered the “Input” of the UAI, as this is the place, where data is extracted from the game and converted to a normalized score between 0-1.<br>
Some basic considerations are shipped with the UAI Creator, but in almost all cases, some user defined considerations need to be implemented.<br>
The consideration uses a **Response Curve** to convert the game data to a normalized score.<br>
A score of 0 means that the consideration is impossible, and that the associated decision/bucket should be invalidated. An example of could be “Do I have a ranged weapon?”, and it could be attached to a bucket containing ranged attacks. If the agent does not have a ranged weapon, there is no reason to consider ranged attacks.<br>
The performance tag is set by the user and is used by the UAI Creator to determine the order of execution of considerations. For performance reasons it is desirable to calculate performance-light considerations before performance-heave considerations.<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/Consideration%20RichPicture.png "Consideration Rich Picture")

There are multiple variants of considerations. The description above applies to all types of considerations, except as stated below.

- Modifier (Derived class)
  - Used to change the weight of the parent decision/bucket.<br>
    The returned score is not used for calculating the parents’ score.<br>
    Example use case: The agent the decision “Heal Self”, which is placed in a combat bucket with a weight of 5. If the agents’ health drops below 5%, the “Heal Self” decision should be considered as life saving instead of basic combat. So, the Consideration-Modifier returns a score of 9 effectively raising the weight of the “Heal Self” decision.<br>
- Boolean (Derived class)
  - Used for simple questions which could invalidate the decision.<br>
    The returned score is not used for calculating the parents’ score.<br>
    The evaluation of consideration-booleans, are prioritized over basic considerations with the same performance tag, because these considerations have a high chance of invalidating the decision, thereby saving CPU time, by not evaluating redundant considerations.<br>
    Consideration-booleans aren’t part of the parents score because their success value is always 1, which means, that a decision with many Boolean considerations would score higher than intended.<br>
    An example is: “Do I have a Ranged Weapon?”, which has the potential to negate all ranged attack, but if the agent has a ranged weapon, doesn’t say anything about the agents’ utility of using the ranged weapon.<br>
- Setter (Attribute - _Can be set on any type of consideration_)
  - Indicates, that the consideration saves data in the AI Context.<br>
    Setters must be evaluated before any other considerations since other considerations might depend on them.<br>
    A use case is: For an agent to move he must consider: “Clear path to target?”, “Time to travel path”, and “Threats on the path”. Individually these considerations would need to calculate the path to the target. Which probably is a performance heavy operation. To avoid this the “Clear path to target?” considerations, finds the path, and stores it in the AI Context, for the other considerations to use.<br>
 
##### Code

When implementing a consideration, the CalculateBaseScore method should be overridden. This method should return a float, representing the considered game data. The response curve will then convert the returned value to a normalized score.<br>
Other methods to implement could be: BaseScoreBelowMinValue and BaseScoreAboveMaxValue. These defines, what the consideration should return if the game data was outside the defined range.<br>
Sample code can be found in [UAC-Show Case](https://github.com/DevOelgaard/UAC-Showcase)<br>

#### Response Curve

The Response Curve is used to define how game data should be converted.<br>
In the example on the screenshot of the Response Curve UI, an agent has an automatic rifle. At close range the rifle shouldn’t be used and returns a value of 0. The rifle is most effective at medium range, which is represented by the normal distribution between X=0.1 and X=0.8. Finally, the rifle has some special ability, which is very effective at long ranges.<br>
You can create your own response functions by creating a class, that inherits from ResponseFunction.<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/Screenshot%20ResponseCurve.png "ResponseCurve - Screenshot")

#### AgentAction

The agent action can be considered the “Output” of the UAI.<br>
The UAI Creator comes with some Unity specific Agent Actions, but in most cases, they should be implemented by the user.<br>

##### Code

The following functions should be used, when implementing an agent action.<br>
- OnStart – Executed the first time an action is selected.<br>
- OnGoing – Executed on each consecutive selection.<br>
- OnEnd – Executed when the action was selected during last tick, but not selected this tick.<br>

| This evaluation | Last evaluation | Method  |
|:---------------:|:---------------:|:--------|
|    Selected     |       Not       | OnStart |
|    Selected     |    Selected     | OnGoing |
|       Not       |    Selected     | OnEnd   |

Sample code can be found in [UAC-Show Case](https://github.com/DevOelgaard/UAC-Showcase)<br>

#### AiContext

Used to share data between UAI Objects.<br>
All UAI Objects has read/write access to the context.<br>

##### Code

The relevant fields and methods are:<br>
- object GetContext(object key, AiObjectModel requester), the key specifies, which data to access. The requester is the requesting UAI Object. If the requester is null, data will be searched from global available data. If the requester is a UAI Object, data will be searched in the context of the requester, all its ancestors and globally.<br>
- Void SetContext(object key, object value, AiObjectModel owner), sets a value for a given key, and specifies the owner of the data. If no owner is set the data is stored globally. Define the owner when you want the data to be shared between descendants.<br>
- LastSelectedDecision, LastSelectedBucket, CurrentEvaluatedDecision and CurrentEvaluatedBucket are used to access the bucket and decision who “won”, the last tick, and the bucket and decision, which is currently being evaluated.<br>

```csharp
    {

        var dataToShare = "Some data";
        
        // Storing data for sharing
        context.SetContext("SOME KEY",dataToShare,Parent);

        // Retrieving data 
        var retrievedData = context.GetContext<string>("SOME KEY", this);
    }
```

Sample code can be found in [UAC-Show Case](https://github.com/DevOelgaard/UAC-Showcase)<br>

#### AgentMono

The AgentMono component allows monobehaviours to use the UAI. Attach it to a monobehaviour through the Unity Inspector and set the settings as you wish.<br>
The settings are:<br>
- Default Ai: Sets the default AI to be selected on game start (Identified by name). If an error occurs, and the UAI can’t be found at game start, the first playable UAI will be selected.<br>
- Auto Tick: If enabled the agent automatically makes decisions. This should be used for most games, that isn’t turn based or requires an agent to only act as a response to an event.<br>
- Ms Between Ticks: Set the number of milliseconds that must have passed before the next decision.<br>
- Frames Between Ticks: Set the number of frames that must have passed before the next decision.<br>

The _Ms Between Ticks_ and _Frames Between Ticks_ are equal to a logical AND, meaning, that both must be true before a new Tick can be selected.<br>

Use the … between ticks, settings to improve performance, by limiting the resources spent on decision making for an agent.<br>
This is very use case specific. In an FPS or racing game the user might expect the agents, to react instantly to the users’ inputs, and these should be set very low. In other games it might seem weird, if all agents act instantaneously to changes in the game world.<br>
![img](https://github.com/DevOelgaard/UnityUtilityAiSystem/blob/784ead147036c3ce9835603de74324bbb7076e69/ReadmeResources/Images/AgentMono%20Inspector.png "AgentMono Inspector")

#### Parameters

The old Parameters.cs has been **deprecated**. <br>

_Description for the new parameter types must be filled here_. Until then you refer to the coding examples<br>

Multiple Types of parameters exists: 
- ParamBool
- ParamColor
- ParamEnum
- ParamFloat
- ParamInt
- ParamString

Add parameters in the constructor of the desired AiObjectModel (_Uai, Bucket, Decision, Consideration, AgentAction, Response Function_).<br>
Get the parameters as needed.
```csharp
using UnityEngine;

public class Demo_AllParametersAction: AgentAction
{
    public Demo_AllParametersAction()
    {
        // Parameters must be added in the constructor
        AddParameter("Bool", true);
        AddParameter("Enum",PerformanceTag.High);
        AddParameter("Float",1f);
        AddParameter("Int",100);
        AddParameter("String","Hi you");
        AddParameter("Color",Color.blue);
    }

    public override void OnStart(IAiContext context)
    {
        var b = ParameterContainer.GetParamBool("Bool");
        var c = ParameterContainer.GetParamColor("Color");
        var e = ParameterContainer.GetParamEnum("Enum");
        var f = ParameterContainer.GetParamFloat("Float");
        var i = ParameterContainer.GetParamInt("Int");
        var s = ParameterContainer.GetParamString("String");
    }
}
```

#### Decision Making Objects (DMO)

Decision Making Objects, references the objects, which in union handles the decision-making logic.<br>

##### Decision Score Evaluator (DSE)

The DSE is the main DMO and serves as an interface to the underlying DMOs. The DSE is responsible for the continuous evaluation, until the best set of Agent Actions has been found.
In most cases you should forget about this one and leave it as it is.

##### Utility Container Selector (UCS)

Any UAI has two UCS’s one for selecting buckets (Bucket Selector) and one for selecting decisions (Decision Selector). They can be changed from the UI (Template Manager).<br>
There are many ways to select the next utility container (bucket or decision). The UAI Creator comes with the following UCS options:<br>
- Highest Score: Selects the highest scoring utility container.
- Random X Highest: Select a random utility container among the X highest scorers. Can be set to select a random utility, where the chance of being selected is relative to the score (Percentage chance). Can set a max allowed deviation from the highest scorer, so only decisions which appears logical to the user is considered.<br>
You can add your own UCS by creating a class that inherits from UtilityContainerSelector and implementing the abstract methods.<br>

#### Utility Scorer

Is responsible for calculating the utility from a set of considerations. Currently the UAI Creator comes with the following utility scorers:<br>

- Average scorer: Returns the average score of all considerations for a given utility container.<br>
- Highest score: Returns the highest score of all considerations for a given utility container.<br>
- Lowest score: Returns the lowest score of all considerations for a given utility container.<br>
You can add your own Utility Scorer by creating a class, that implements the IUtilityScorer, but remember:<br>
- Return 0 if any consideration returns <=0.<br>
- Only use the score from considerations which are scorers
  - Use Consideration.IsScorer to check this. _(See advanced guide)_

## Sponsor - Keep the project alive

[![](https://img.shields.io/static/v1?label=Sponsor&message=%E2%9D%A4&logo=GitHub&color=%23fe8e86)](https://github.com/sponsors/DevOelgaard)<br>

Becoming a sponsor will help this project stay alive, and make me a happy man :).
