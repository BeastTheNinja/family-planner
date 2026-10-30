export interface FamilyTask {
    id: number;
    title: string;
    isCompleted: boolean;
}

export interface FamilyMember {
    id: number;
    name: string;
    isActive: boolean;
    tasks: FamilyTask[];
}