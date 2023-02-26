using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact(Action onInteractComplete); //Given that this is interface we do not have to specify because it has to be public
}
