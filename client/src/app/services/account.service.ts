import { inject, Injectable } from "@angular/core";
import { UserApiService } from "../api-client";
import { lastValueFrom } from "rxjs";

@Injectable()
export default class AccountService {
    
    private api = inject(UserApiService);

    public user: string | null = null;

    async identity() {
        this.user = await lastValueFrom(this.api.apiUserMeGet())
    }

    clear() {
        this.user = null;
    }

    

}