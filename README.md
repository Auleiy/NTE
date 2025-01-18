# Project File
## TCF (Turtle Config File)
TCF is a json file, has many types like background config, character config, ui config, etc. But they all have some fixed structure object.

### Fixed structure objects
#### Vector
Vector is represented as array. Array's length indicates vector's dimension. It usually is 2D (2 dimensions) that many arrays has 2 values. Each value corresponds to the coordinates of a dimension.

Example:
```JSON
"Position": [34, 21],
"Rotation": [20, 90, 30]
```
It means `"Position"` is $(34, 21)$ and `"Rotation"` is $(20, 90, 30)$ (Only z-axis value will be required in rotation).

#### Image
Image storages its path and transform data. Path should be a relative path example as "/Sprites/Test.png". It can only be PNG format, even not supports JPEG format.

Structure:
```JSON
{
    "Path": "PATH",
    "Pivot": [x, y],
    "Clipping": [l, t, r, b]
}
```

`"Path"` is a string, indicates the path of the image.

`"Pivot"` is a 2D vector, indicates its pivot in pixel from bottom left to top right.

`"ClippingSize"` is a 4D vector, indicates its clipping size in pixel. $l + r \le Width$ and $t + b \le Height$.

It will clip like figure 1

![Clipping diagram](README/clipping.png)

### Character Config
```JSON
{
    "DefaultName": "NAME",
    "Bases": [
        {
            "Name": "POSE_NAME",
            "Image": {
                "Path": "/Sprites/NAME/Poses/POSE_NAME.png",
                "Pivot": [64, 256],
                "Clipping": [0, 0, 0, 32]
            }
        }
    ],
    "Faces": [
        {
            "Name": "FACE_NAME",
            "Image": {
                "Path": "/Sprites/NAME/Faces/FACE_NAME.png",
                "Pivot": [32, 384],
                "Clipping": [0, 0, 0, 0]
            }
        }
    ]
}
```
It has DefaultName, Bases and Emotions config.

`"DefaultName"` is a string, name of character in default. To be a translatable text, use `"#tr:{Translatable key}"`. It can replaced in all type of texts, like scenario name. Use this format: `"#ch:{File name without extension}"`.

#### Bases
It is an array, storaged character bases (like poses). It dosen't have facial features in order to optimize.

`"Name"` is a string, like internal id, will be called in command `chbase` to change pose.

`"Image"` is the image of the pose.

#### Faces
It is also an array, stroaged character faces. It filled the facial features lost by the base. It's better only save the changed pixels instead of a complete image that is exactly the same except the face.





## SCN (Turtle Scenario File)
SCN is the main language in the project. It looks like a command-line scripting language. Even this language uses that style, it has a completely usage.

The game will read the specified first scenario file. Then, it runs from top to bottom. Some command like `say`, `wait` will block the game process.

Don't worry. It won't block all time and the player won't stuck at that time. Say will just waiting for players' click (except auto mode) because player needs time to read it. Wait is just a timing command to bring a better visual experience. The above commands are mentioned below.

### Language Style
*I'm sorry that i can't make a syntax highlight for my language.*

As mentioned above, this language is in the style of a command-line scripting language. But in case some people don't know, and present the language features that that style dosen't have, we'll explain the language style.

```
say chName:"Character", msg:""MessageMessageMessage.
MessageMessageMessage""

set varName:VariableName, value:Value

say "Character", ""The VariableName is {Value}.""
jump chapter:0 fileName:"0.scn"
```

The code above is in this language. Most of them are like that style but it has a big difference. **This language is case-sensitive.**

In this language, you can add a variable name before parameter to make parameters in no sequence and skip parameters. Use colons to separate. Each parameter separated by a comma. You can add space after and before it. Parser won't be influenced by it.

#### String
String uses as most language. It starts and ends with quote. If uses two quote to start or end, it will be a wrappable text and it can write inline variables in the format `"{VariableName}"` by the `set` command.

#### Raw text
Raw text is a string without quote to start and end, so it even can't have spaces, colons and more.

#### Number
The numbers are exactly the same as in other languages, except the prefix and suffix to explain the number in other base number or other type of number. 

The numbers are divided into Integer type and Float type. Each number is auto parsed in the code that you can't write a fraction number for a integer parameter.

#### Boolean
The boolean values are also exactly the same as in other languages. `true` means True, On, OK, Yes, etc. and `false` means the opposite.

### Command references
```
The parameter with * means that it must be provided.

set
    Set a variable. If it dosen't exists in the context, it will create a variable.
    Arguments:
        *varName: The name of variable : RawText
         value: The value of variable : Any

say
    Make character say a message.
    Arguments:
        *chName: The name of character : String
        *msg: The message : String

think
    Make current view think a message or aside.
    Arguments:
        *msg: The message : String

ch_show
    Show a character.
    Arguments:
        *id: The character id in the context : String
        *cfg: The character config path : String
        *posX: The X coordinate of the initial position : Float
        *posY: The Y coordinate of the initial position : Float
         rotZ: The character's initial rotation : Float, Default: 0
         colR: The red value of the initial color in 0-1 : Float, Default: 1
         colG: The green value of the initial color in 0-1 : Float, Default: 1
         colB: The blue value of the initial color in 0-1 : Float, Default: 1
         colA: The alpha value of the initial color in 0-1 : Float, Default: 1
         trans: The transition of the showing animation : Float, Default: 0
         ease: The ease of the showing animation : Ease, Default: Linear

ch_layer
    Set a layer state of character.
    Arguments:
        *id: The character id in the context : String
        *name: The layer name : String
         on: The layer's state : Boolean, Default: <Toggled Value>
         trans: The transition : Float, Default: 0
         ease: The ease : Ease, Default: Linear

ch_move
    Move a character.
    Arguments:
        *id: The character id in the context : String
         posX: The target position X : Float, Default: <Original Value>
         posY: The target position Y : Float, Default: <Original Value>
         trans: The transition : Float, Default: 0
         ease: The ease : Ease, Default: Linear

ch_rot
    Rotate a character.
    Arguments:
        *id: The character id in the context : String
         rotZ: The target rotation : Float, Default: <Original Value>
         trans: The transition : Float, Default: 0
         ease: The ease : Ease, Default: Linear

ch_col
    Stain a character (I'm not kidding).
    Arguments:
        *id: The character id in the context : String
         colR: The target color R in 0-1 : Float, Default: <Original Value>
         colG: The target color G in 0-1 : Float, Default: <Original Value>
         colB: The target color B in 0-1 : Float, Default: <Original Value>
         colA: The target color A in 0-1 : Float, Default: <Original Value>
         trans: The transition : Float, Default: 0
         ease: The ease : Ease, Default: Linear

cg_show
    Show a CG/Background.
    Arguments:
        *id: The CG/Background id in the context : String
         focus: Zoom and blur to analog focus on the fore object : Boolean, Default: false
         colR: The target color R in 0-1 : Float, Default: <Original Value>
         colG: The target color G in 0-1 : Float, Default: <Original Value>
         colB: The target color B in 0-1 : Float, Default: <Original Value>
         trans: The transition : Float, Default: 0
         ease: The ease : Ease, Default: Linear
```
# Annex
## Enum Types
### Ease
- Linear
- InSine
- OutSine
- InOutSine
- InQuad
- OutQuad
- InOutQuad
- InCubic
- OutCubic
- InOutCubic
- InQuart
- OutQuart
- InOutQuart
- InQuint
- OutQuint
- InOutQuint
- InExpo
- OutExpo
- InOutExpo
- InCirc
- OutCirc
- InOutCirc
- InElastic
- OutElastic
- InOutElastic
- InBack
- OutBack
- InOutBack
- InBounce
- OutBounce
- InOutBounce
- Flash
- InFlash
- OutFlash
- InOutFlash