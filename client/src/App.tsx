import { useEffect, useState } from "react";
import type { FamilyMember } from "./types/family";

function App() {
  const [members, setMembers] = useState<FamilyMember[]>([]);
  const [newMemberName, setNewMemberName] = useState<string>("");
  const [isCreating, setIsCreating] = useState<boolean>(false);
  const [createError, setCreateError] = useState<string | null>(null);
  const [isLoadingMembers, setIsLoadingMembers] = useState<boolean>(true);
  const [membersError, setMembersError] = useState<string | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const [editingMemberId, setEditingMemberId] = useState<number | null>(null);

  useEffect(() => {
    async function fetchMembers() {
      setIsLoadingMembers(true);
      setMembersError(null);
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
        setMembersError("Could not fetch family members. Please try again.");
        console.error("Error fetching family members:", error);
      } finally {
        setIsLoadingMembers(false);
      }
    }

    fetchMembers();
  }, []);

  async function handleAddMember() {
    if (newMemberName.trim() === "") {
      return;
    }
    setCreateError(null);
    setIsCreating(true);
    try {
      const response = await fetch(
        "http://localhost:5084/api/familymembers",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            name: newMemberName.trim(),
          }),
        }
      );

      if (!response.ok) {
        throw new Error("Failed to create family member");
      }

      const createdMember: FamilyMember = await response.json();

      setMembers((prevMembers) => [...prevMembers, createdMember]);

      setNewMemberName("");
    } catch (error) {
      setCreateError("Could not create family member. Please try again. ");
      console.error("Error creating family member:", error);
    } finally {
      setIsCreating(false);
    }
  }
  async function handleDeleteMember(id: number) {
    setDeleteError(null);
    try {
      const response = await fetch(
        `http://localhost:5084/api/familymembers/${id}`,
        {
          method: "DELETE",
        }
      );

      if (response.status === 409) {
        throw new Error(
          "Cannot delete a family member who has assigned tasks."
        );
      }

      if (!response.ok) {
        throw new Error("Failed to delete family member");
      }

      setMembers((prevMembers) =>
        prevMembers.filter((member) => member.id !== id)
      );
    } catch (error) {
      if (error instanceof Error) {
        setDeleteError(error.message);
      } else {
        setDeleteError("An unexpected error occurred.");
      }

      console.error("Error deleting family member:", error);
    }
  }

  return (
    <main>
      <h1>Family Planner</h1>
      <p>Editing member ID: {editingMemberId ?? "None"}</p>
      <div>
        <input
          type="text"
          placeholder="Enter family member name"
          value={newMemberName}
          onChange={(e) => setNewMemberName(e.target.value)}
        />
        <p>Du skriver: {newMemberName}</p>
        <button
          type="button"
          onClick={handleAddMember}
          disabled={isCreating}
        >
          {isCreating ? "Creating..." : "Add Member"}
        </button>
        {createError && (
          <p role="alert" style={{ color: "red" }}>
            {createError}
          </p>
        )}
      </div>
      {isLoadingMembers && <p>Loading family members...</p>}
      {membersError && (
        <p role="alert" style={{ color: "red" }}>
          {membersError}
        </p>
      )}
      {!isLoadingMembers && !membersError && members.length === 0 && (
        <p>No family members found.</p>
      )}
      {deleteError && (
        <p role="alert" style={{ color: "red" }}>
          {deleteError}
        </p>
      )}
      <ul>
        {members.map((member) => (
          <li key={member.id}>
            <button
              type="button"
              onClick={() => setEditingMemberId(member.id)}
            >
              Edit
            </button>
            <button
              type="button"
              onClick={() => handleDeleteMember(member.id)}
            >
              Delete
            </button>
            {member.name}
            <ul>
              {member.tasks.map((task) => (
                <li key={task.id}>
                  {task.title}
                  {task.isCompleted ? " (Completed)" : " (Not Completed)"}

                </li>
              ))}
              {member.tasks.length === 0 && <li>Ingen opgaver</li>}
            </ul>
          </li>
        ))}
      </ul>
    </main>
  );
}

export default App;