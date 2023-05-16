using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Persona Database",menuName ="AI/Database/Personas")]
public class PersonaDatabase : ScriptableObject
{


    public AIPersonality[] Personas;
}
