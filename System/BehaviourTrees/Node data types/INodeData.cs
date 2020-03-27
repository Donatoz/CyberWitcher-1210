namespace VovTech.Behaviours
{
    /// <summary>
    /// Delegate for calling the exit event.
    /// </summary>
    /// <param name="childId">Next node local id.</param>
    public delegate void ExitState(int childId);

    /// <summary>
    /// Interface for all node data.
    /// </summary>
    public interface INodeData
    {
        /// <summary>
        /// Node name.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Node global id.
        /// </summary>
        int Id { get; set; }
        /// <summary>
        /// Node which this data is attached to.
        /// </summary>
        Node AttachedNode { get; }
        /// <summary>
        /// Attach this data to some node.
        /// </summary>
        /// <param name="node">Node-holder of this data.</param>
        void Attach(Node node);
    }
}