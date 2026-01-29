export interface Cell
{
    x: number;
    y: number;
    type: string;
    hasTurbine: boolean;
    powerOutput?: number;
    modifiedWindSpeed?: number;
    modifiers?: string[];
    color?: string;
}
