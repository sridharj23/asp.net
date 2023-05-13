import axios, { type AxiosResponse } from 'axios';

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

    protected handleError(e: unknown, method : string) {
        alert("Error in " + method + " : " + e);
    }

    public async index<T>(path : string) : Promise<T[]> {
        let result : T[] = new Array<T>;
        try {
            return await this.connection.get(path).then(response => response.data as T[]);
        } catch (error) {
            this.handleError(error, "INDEX");
        }
        return result;
    }

    public async single<T>(path: string) : Promise<T> {
        let result : T[] = new Array<T>;
        try {
            return await this.connection.get(path).then(response => response.data as T);
        } catch (error) {
            this.handleError(error, "GET");
        }
        return null as T;
    }

    public async post<T>(path : string, input : T) : Promise<T> {
        try {
            console.log(input);
            return await this.connection.post(path, input).then(response => response.data as T);
        } catch (error) {
            this.handleError(error, "POST");
        }
        return input;
    }

    public async put<T>(path : string, input : T) : Promise<T> {
        try {
            return await this.connection.put(path , input).then(response => response.data as T);
        } catch (error) {
            this.handleError(error, "PUT");
        }
        return input;
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