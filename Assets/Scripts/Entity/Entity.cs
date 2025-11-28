using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EntityStoryAdvancement
{
    public int index;
    public string dialogue;
    public Transform entityLocation;
    //public Animation entityAnimation;
}
[System.Serializable]
public class Entity : MonoBehaviour, Interactable
{
    public List<EntityStoryAdvancement> entityStoryLocations = new List<EntityStoryAdvancement>();

    [SerializeField] private List<string> remarkWhenHoldingAnObject = new List<string>();
    public EntityStoryAdvancement actualEntityStoryLocation { get; private set; }
    private bool firstTimeHoldingAnObject = true;
    private int storyIndex = 0;


    public void Interact()
    {
        GameManager.Instance.NpcInteraction.Invoke(actualEntityStoryLocation.dialogue);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AdvanceStoryEntity()
    {
        actualEntityStoryLocation = entityStoryLocations[storyIndex];
        if(storyIndex < entityStoryLocations.Count -1)
        {
            storyIndex++;
        }
    }

    public void SendRemarkWhenHoldingObject()
    {
        // To be sent when the player hold an object !
        string remark = GenerateRandomRemark();
        GameManager.Instance.NpcInteraction.Invoke(remark);
    }

    private string GenerateRandomRemark()
    {
        if (firstTimeHoldingAnObject)
        {
            firstTimeHoldingAnObject = false;
            return remarkWhenHoldingAnObject[0];
        }
        int randomIndex = Random.Range(0, remarkWhenHoldingAnObject.Count);
        for (int i = 0; i < remarkWhenHoldingAnObject.Count; i++)
        {
            if (i == randomIndex)
            {
                return remarkWhenHoldingAnObject[i];
            }
        }
        return "...";
    }
}
