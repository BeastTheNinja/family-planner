import { useEffect, useState } from "react";
import type { FamilyMember } from "./types/family";

function App() {
  const [members, setMembers] = useState<FamilyMember[]>([]);

  useEffect(() => {
    async function fetchMembers() {
      try {
        const response = await fetch(
          "http://localhost:5084/api/familymembers"
        );

        if (!response.ok) {
          throw new Error("Failed to fetch family members");
        }

        const data: FamilyMember[] = await response.json();
        setMembers(data);
      } catch (error) {
        console.error("Error fetching family members:", error);
      }
    }

    fetchMembers();
  }, []);

  return (
    <main>
      <h1>Family Planner</h1>
      <ul>
        {members.map((member) => (
          <li key={member.id}>
            {member.name}
            <ul>
              {member.tasks.map((task) => (
                <li key={task.id}>
                  {task.title}
                  {task.isCompleted}
                </li>
              ))}
            </ul>
          </li>
        ))}
      </ul>
    </main>
  );
}

export default App;