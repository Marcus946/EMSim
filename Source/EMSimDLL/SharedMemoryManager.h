#ifndef SHAREDMEMORYMANAGER_H
#define SHAREDMEMORYMANAGER_H

#include <windows.h>

#ifdef EMSIMDLL_EXPORTS
#define EMSIMDLL_API extern "C" __declspec(dllexport)
#else
#define EMSIMDLL_API extern "C" __declspec(dllimport)
#endif

namespace SharedMemoryManager
{
    namespace
    {
        // any value representing indexes or sizes of shared memory can always be int instead of size_t (max 2GB).
        // this increases interoperability with the Unity/C# side
        const int SHARED_MEM_SIZE = 1024;
        const char *SHARED_MEM_NAME = "Local\\MySharedMemory";
        const char *MUTEX_NAME = "Local\\MySharedMutex";

        HANDLE mMutex = nullptr;
        HANDLE mMapFile = nullptr;
        LPVOID mFileBuf = nullptr;
    }

    EMSIMDLL_API void Open();

    EMSIMDLL_API void Close();

    EMSIMDLL_API bool Write(
        const void *inputBuf,
        const int length);

    EMSIMDLL_API bool Read(
        void *outputBuf,
        const int bufSize,
        const int offset = 0);

    EMSIMDLL_API bool Valid();

    EMSIMDLL_API constexpr int BufferSize()
    {
        return SHARED_MEM_SIZE;
    }
}

#endif // !SHAREDMEMORYMANAGER_H