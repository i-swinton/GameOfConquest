using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Persona Database",menuName ="AI/Database/Personas")]
public class PersonaDatabase : ScriptableObject
{


    public AIPersonality[] Personas;
}

public static class PersonasDB
{
    static PersonaDatabase db;

    static void LoadDatabase()
    {
        // Load in the database
        db = (PersonaDatabase)Resources.Load("AI/Personalities/Personas");
    }

    public static AIPersonality GetPersonality(int i)
    {
        if (db == null) { LoadDatabase(); }
        return db.Personas[i];
    }

    public static AIPersonality RandomPersonality()
    {
        if(db == null) { LoadDatabase(); }

        return db.Personas[RNG.Roll(0, db.Personas.Length-1)];
    }
}