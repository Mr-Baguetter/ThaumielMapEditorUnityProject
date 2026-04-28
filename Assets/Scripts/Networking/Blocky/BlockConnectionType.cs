namespace Assets.Scripts.Networking.Blocky
{
    public enum BlockConnectionType
    {
        /// <summary>
        /// The small peg on the right plugs into an Input connection.
        /// Used for value blocks that return a result (e.g. a number, Vector3).
        /// Sets the block's Output in Blockly.
        /// </summary>
        Output,

        /// <summary>
        /// The socket on the left accepts an Output peg.
        /// Defined as input_value in the args list.
        /// </summary>
        Input,

        /// <summary>
        /// The notch on the top connects to a Next connection below it.
        /// Used for statement blocks that stack vertically.
        /// </summary>
        Previous,

        /// <summary>
        /// The bump on the bottom connects to a Previous connection above it.
        /// Used for statement blocks that stack vertically.
        /// </summary>
        Next,
    }
}