// 角色屬性
export interface CharacterStats {
  str: number;
  dex: number;
  int: number;
  wis: number;
  con: number;
}

// 裝備加成
export interface EquipmentBonuses {
  atk: number;
  def: number;
  str: number;
  dex: number;
  int: number;
  wis: number;
  con: number;
}

// 完整角色狀態
export interface PlayerFullStats {
  name: string;
  className: string;
  level: number;
  exp: number;
  expRequired: number;
  currentHp: number;
  maxHp: number;
  currentMp: number;
  maxMp: number;
  stats: CharacterStats;
  equipmentBonuses: EquipmentBonuses;
}

// 背包物品
export interface InventoryItemDto {
  id: string;
  name: string;
  type: string;
  icon: string;
  quantity: number;
  isEquipped: boolean;
  equippedSlot: string;
  description: string;
  properties: Record<string, number>;
}

// 背包資料
export interface InventoryData {
  items: InventoryItemDto[];
  gold: number;
}

// 房間資訊
export interface RoomInfo {
  id: number;
  name: string;
  description: string;
  monsters: string[];
}

// 出口資訊
export interface ExitInfo {
  direction: string;
  roomId: number;
  roomName: string;
  hasMonsters: boolean;
}

// 地圖資料
export interface MapData {
  currentRoom: RoomInfo;
  exits: ExitInfo[];
}

// 技能資訊
export interface SkillDto {
  skillId: string;
  name: string;
  description: string;
  type: string;
  mpCost: number;
  requiredLevel: number;
  isLearned: boolean;
}

// 技能資料
export interface SkillsData {
  learnedSkills: SkillDto[];
  lockedSkills: SkillDto[];
}

// 遊戲訊息類型
export type MessageType = 'combat' | 'general' | 'system';

// 遊戲訊息
export interface GameMessage {
  user: string;
  content: string;
  type: MessageType;
  timestamp: Date;
}
