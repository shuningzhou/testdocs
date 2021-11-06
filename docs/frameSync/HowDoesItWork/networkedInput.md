# **Networked Input**

Collecting and distributing player inputs and make they accessible during runtime is a main feature
Before the frame simulation starts, the `FrameSyncEngine` reads the input data of the frame and assign the input values to the players. 

During a frame simulaiton, the `FrameSyncEngine` updates the `FrameSyncBehaviour`s and your IFrameSyncPlayerUpdate MonoBehaivours will be updated. The player that owns the `FrameSyncBehaviour` will be passed into the `OnPlayerUpdate()` method. You can read the inputs of the player by calling `player.GetInput{InputName}()`;

???+ info

    The `player.GetInput{InputName}()` method is generated when you save the InputSettings of the game.
    See [InputSettings][6] for more details.

## **Example Script Showing The Seperation of Game Logic And Presentation**
=== "C#"
    ``` c#
    public class MyExampleScript : MonoBehaviour, IFrameSyncOnStart, IFrameSyncPlayerUpdate
    {
        //Displays player's score in a label, not used in the Game Logic
        public Text playerScoreLabel;

        //the player score, this value is used in the Game Logic
        int playerScore

        //==========Unity Events==========
        //only used to read playerScore and update the playerScoreLabel
        void Update()
        {
            playerScoreLabel.text = playerScore.ToString();
        }

        //==========Game Logic==========
        //IFrameSyncOnStart
        //used for initialize the component
        public void OnStart(FrameSyncBehaviour frameSyncBehaviour)
        {
            playerScore = 0;
        }

        //IFrameSyncPlayerUpdate
        //used to read player input and change player score
        public void OnPlayerUpdate(FrameSyncPlayer player, FrameSyncGame game, FrameSyncUpdateType frameSyncUpdateType)
        {
            bool addScore = player.GetInputAddScore();

            if(addScore)
            {
                playerScore++;
            }
        }
    }
    ```