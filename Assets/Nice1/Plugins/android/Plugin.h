#pragma once

#if _MSC_VER
#define EXPORT_API __declspec(dllexport)
#else
#define EXPORT_API
#endif

// ------------------------------------------------------------------------
#include <iostream>
#include <stdio.h>
#include <string>
#include <cstring>
#include "curl/curl.h"
#include "json.h"
#include "json.c"
#include "JsonDataContainer.h"

using namespace quicktype;
using namespace std;
using json = nlohmann::json;

extern "C"
{
	size_t WriteCallback(void* contents, size_t size, size_t nmemb, void* userp);

	// This function takes a character string and creates a dynamic copy
	// of it in memory, returning a pointer to the new block of memory
	char* MakeStringCopy(const char* string);

	string networkEndpoints[] = {
		"http://jungle4.greymass.com/v1/chain/get_table_rows", // Jungle4 Testnet
		"https://eos.greymass.com/v1/chain/get_table_rows", // EOS Mainnet
		"https://protontestnet.greymass.com/v1/chain/get_table_rows", // Proton Testnet
		"https://proton.cryptolions.io/v1/chain/get_table_rows", // Proton Mainnet
		"https://testnet.waxsweden.org/v1/chain/get_table_rows", // WAX Testnet
		"https://wax.greymass.com/v1/chain/get_table_rows", // WAX Mainnet
		"https://test.telos.eosusa.io/v1/chain/get_table_rows", // TELOS Testnet
		"https://telos.greymass.com/v1/chain/get_table_rows" // TELOS Mainnet
	};

	bool simpleAssets;

	const char* CURL_INIT_ERROR_MESSAGE = "CURL_INIT_ERROR";
	const char* CURL_REQUEST_ERROR_MESSAGE = "CURL_REQUEST_ERROR";

	const char* GetUrl(const char* ownerChar, const char* authorChar, const char* categoryChar, int network);

	const char* GetCurlResponse(const char* url, const char* owner);

	bool IsValid(JsonNode* node) { return node != nullptr && node; }

	const char* GetAuthorNameSimpleAssets(JsonNode* element);

	const char* GetAuthorNameDeltas(JsonNode* element);

	const char* GetLicenseFromJsonData(const char* curlResponse, const char* licenseCheck, char* category);

	const char* CheckLicense(char* owner, char* author, char* category, char* license_name, int network);

	const char* CheckNice1GenesisKey(char* owner, int network);

	EXPORT_API const char* CheckLicensePlugin(char* owner, char* author, char* category, char* license_name, int checkNice1GenesisKey, int network);

	EXPORT_API int APITest();
}