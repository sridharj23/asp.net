import axios from 'axios';
import { useStatusStore } from '@/stores/statusstore';

export abstract class RestApi {
    readonly connection;
    protected static readonly apiBase:string = `/api/`;

    constructor() {
        this.connection = axios.create({
            baseURL: RestApi.apiBase,
            timeout: 300000,
            headers: { 'Content-Type': 'application/json' }
        });
    };

    protected getQueryString(params?: Map<string,string>) : string {
        let qs = new URLSearchParams("");
        if (params != undefined) {
            params.forEach((value:string, key:string) => {
                qs.append(key, value);
            })
        }
        let str = qs.toString();
        if (str) {
            str = "?" + str;
        }
        return str;
    }

    public handleError(e: unknown, method : string) {
        console.log(e);
        let store = useStatusStore();
        store.setError("Error executing " + method + " : " + e)
        throw e;
    }

    public displayInfo(msg : string) {
        let store = useStatusStore();
        store.setInfo(msg);
    }

    public async index<T>(path : string) : Promise<T[]> {
        let result : T[] = new Array<T>;
        await this.connection.get(path).then(response => result = response.data as T[]).catch(err => this.handleError(err, "INDEX"));
        return result;
    }

    public async single<T>(path: string) : Promise<T> {
        let result : T = null as T;
        await this.connection.get(path).then(response => result = response.data as T).catch(err => this.handleError(err, "GET"));
        return result;
    }

    public async post<T>(path : string, input : T) : Promise<T> {
        let result : T = input;
        await this.connection.post(path, input).then(response => result = response.data as T).catch(err => this.handleError(err, "POST"));
        return result;
    }

    public async put<T>(path : string, input : T) : Promise<T> {
        let result : T = input;
        await this.connection.put(path , input).then(response => result = response.data as T).catch(err => this.handleError(err, "PUT"));
        return result;
    }

    public async delete<T>(path : string) : Promise<boolean> {
        try {
            return await this.connection.delete(path).then(response => response.data);
        } catch (error) {
            this.handleError(error, "DELETE");
        }
        return false;
    }
}