// 裝備品質等級
export const ItemQuality = {
  Common: 0,    // 白色 - 普通
  Uncommon: 1,  // 綠色 - 精良
  Rare: 2,      // 藍色 - 稀有
  Legendary: 3  // 紫色 - 傳說
} as const;

export type ItemQuality = typeof ItemQuality[keyof typeof ItemQuality];

// 品質顏色對應
export const QualityColors = {
  [ItemQuality.Common]: '#9CA3AF',     // 灰色
  [ItemQuality.Uncommon]: '#22C55E',   // 綠色
  [ItemQuality.Rare]: '#3B82F6',       // 藍色
  [ItemQuality.Legendary]: '#A855F7'   // 紫色
} as const;

// 品質名稱對應
export const QualityNames = {
  [ItemQuality.Common]: '普通',
  [ItemQuality.Uncommon]: '精良',
  [ItemQuality.Rare]: '稀有',
  [ItemQuality.Legendary]: '傳說'
} as const;

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

// 套裝加成（與裝備加成分離）
export interface SetBonuses {
  atk: number;
  def: number;
  str: number;
  dex: number;
  int: number;
  wis: number;
  con: number;
}

// 啟用的套裝資訊
export interface ActiveSet {
  setName: string;
  equippedPieces: number;
  totalPieces: number;
  activeBonuses: {
    requiredPieces: number;
    description: string;
  }[];
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
  setBonuses: SetBonuses;
  activeSets: ActiveSet[];
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
  quality: ItemQuality;
  setId: number | null;
  setName: string | null;
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

// 套裝資訊
export interface EquipmentSetDto {
  id: number;
  name: string;
  description: string;
  totalPieces: number;
  bonuses: SetBonusDto[];
  pieces: SetPieceDto[];
}

// 套裝加成
export interface SetBonusDto {
  requiredPieces: number;
  description: string;
  bonuses: Record<string, number>;
  isActive: boolean;
}

// 套裝部件
export interface SetPieceDto {
  itemId: number;
  name: string;
  isOwned: boolean;
  isEquipped: boolean;
}

// 啟用的套裝加成
export interface ActiveSetBonusesDto {
  activeSets: ActiveSetDto[];
}

// 單一啟用套裝資訊
export interface ActiveSetDto {
  setName: string;
  equippedPieces: number;
  totalPieces: number;
  activeBonuses: ActiveBonusDto[];
}

// 單一啟用加成
export interface ActiveBonusDto {
  requiredPieces: number;
  description: string;
}
