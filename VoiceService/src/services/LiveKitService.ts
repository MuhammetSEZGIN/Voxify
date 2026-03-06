import { AccessToken } from "livekit-server-sdk";
import config from "../config";

export interface ILiveKitService {
  generateRoomToken(
    roomId: string,
    userId: string,
    userName: string
  ): Promise<string>;
}

export class LiveKitService implements ILiveKitService {
  private readonly apiKey: string;
  private readonly apiSecret: string;

  constructor() {
    this.apiKey = config.livekit.apiKey;
    this.apiSecret = config.livekit.apiSecret;
  }

  /**
   * Kullanıcının belirtilen ses odasına katılması için
   * LiveKit uyumlu bir Access Token (JWT) üretir.
   */
  async generateRoomToken(
    roomId: string,
    userId: string,
    userName: string
  ): Promise<string> {
    if (!roomId || !userId || !userName) {
      throw new Error("roomId, userId and userName are required");
    }

    const token = new AccessToken(this.apiKey, this.apiSecret, {
      identity: userId,
      name: userName,
      // Token 6 saat geçerli
      ttl: "6h",
    });

    token.addGrant({
      roomJoin: true,
      room: roomId,
      canPublish: true,
      canSubscribe: true,
      canPublishData: true,
    });

    const jwt = await token.toJwt();
    return jwt;
  }
}
