const API_BASE_URL = "http://localhost:5000/api";

export type Profile = {
  id: string;
  email: string;
  fullname: string;
    phoneNumber: string;
    role: string;
    createdAt: string;
    updatedAt: string;
    addresses: Address[];
};

export type Address = {
  id: number;
  recipientName?: string;
  phoneNumber?: string;
    addressLine1?: string;
  addressLine2?: string;
  city?: string;
  country?: string;
  isDefault?: boolean;
    state: string;
    postalCode: string;
    createdAt: string;
};

export async function getProfile(token: string) : Promise<Profile> {
  try {
    const res = await fetch(`${API_BASE_URL}/profile`, {
        method: "GET",
        headers: {
        "Authorization": `Bearer ${token}`,
      },
      
      cache: "no-store",
    });
    const result = await res.json();
    
    return result.data as Profile;
  } catch (error) {
    console.error("Error in getProfile:", error);
    throw error;
  }
}

export type UpdateProfileAllPayload = {
  fullname?: string;
  phoneNumber?: string;
  newEmail?: string;
  newPassword?: string;
  confirmPassword?: string;
  currentPassword?: string;
};

export async function updateProfileAll(token: string, payload: UpdateProfileAllPayload): Promise<any> {
  try {
    const res = await fetch(`${API_BASE_URL}/profile/all`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(payload),
    });

    const result = await res.json();
    if (!res.ok) {
      throw new Error(result?.message || `Request failed: ${res.status}`);
    }
    return result;
  } catch (error) {
    console.error("Error in updateProfileAll:", error);
    throw error;
  }
}


